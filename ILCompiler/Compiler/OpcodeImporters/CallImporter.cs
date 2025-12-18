using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.Inlining;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.Dnlib;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.OpcodeImporters
{
    public class CallImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, IImporter importer)
        {
            switch (instruction.Opcode)
            {
                case ILOpcode.call:
                case ILOpcode.callvirt:
                    ImportCall(instruction, importer);
                    return true;

                default:
                    return false;
            }
        }

        public static void ImportCall(Instruction instruction, IImporter importer, StackEntry? newObjThis = null)
        {
            var method = (MethodDesc)instruction.Operand;
            ImportCall(method, instruction, importer, newObjThis);
        }

        public static void ImportCall(MethodDesc method, Instruction instruction, IImporter importer, StackEntry? newObjThis = null)
        {
            MethodDesc methodToCall = method;
            var opcode = instruction.Opcode;

            var arguments = new List<StackEntry>();
            for (var i = methodToCall.Signature.Length - 1; i >= 0; i--)
            {
                var argument = importer.Pop();

                if (argument.Type == VarType.Struct)
                {
                    argument = NormalizeStructValue(argument, importer);
                }

                var parameter = methodToCall.Signature[i];
                var parameterType = parameter;

                var parameterVarType = parameterType.Type.VarType;
                if (parameterVarType.IsSmall())
                {
                    /*
                    if (argument is LocalVariableEntry localVariableEntry)
                    {
                        // Not need for PutArgTypeEntry here but need to override sign extend on local variable entry
                        localVariableEntry.OverrideSignExtend = true;
                    }
                    else
                    {
                    */
                        argument = new PutArgTypeEntry(parameterVarType, argument);
                    //}
                }

                arguments.Add(argument);
            }

            if (methodToCall.IsArrayAddressMethod())
            {
                // Ptr to typedesc for array type
                var arrayTypeDescPtr = new NativeIntConstantEntry(importer.NameMangler.GetMangledTypeName(methodToCall.OwningType));
                arguments.Add(arrayTypeDescPtr);
            }

            // Add the this pointer if required, e.g. if part of newobj
            if (newObjThis != null)
            {
                arguments.Add(newObjThis);
            }
            else if (methodToCall.HasThis)
            {
                var thisPtr = importer.Pop();
                arguments.Add(thisPtr);
            }
            arguments.Reverse();

            if (opcode == ILOpcode.callvirt)
            {
                if (importer.Constrained != null)
                {
                    TypeDesc constrained = importer.Constrained;
                    if (constrained.IsRuntimeDeterminedSubtype)
                        constrained = constrained.ConvertToCanonForm(TypeSystem.Canon.CanonicalFormKind.Specific);

                    var constrainedType = constrained.Context.GetClosestDefType(constrained);
                    TypeDesc owningType = method.OwningType;
                    MethodDesc? directMethod = constrainedType.TryResolveConstraintMethodApprox(owningType, method);

                    if (directMethod is not null)
                    {
                        methodToCall = directMethod;
                        opcode = ILOpcode.call;
                    }
                    else if (constrained.IsValueType)
                    {
                        // Dereference this ptr and box it
                        var dereferencedThisPtr = DereferenceThisPtr(arguments[0], constrainedType);
                        var boxedNode = BoxImporter.BoxValue(dereferencedThisPtr, constrainedType, importer);
                        arguments[0] = boxedNode;
                    }
                    else
                    {
                        // Dereference this ptr
                        arguments[0] = DereferenceThisPtr(arguments[0], constrainedType);
                    }

                    importer.Constrained = null;
                }

                // Turn first argument into throw null ref check
                arguments[0] = new NullCheckEntry(arguments[0]);
            }

            if (methodToCall.OwningType.IsInterface || (opcode == ILOpcode.callvirt && methodToCall.IsVirtual))
            {
                // Need to add this pointer as extra param which will be consumed by InterfaceCall routine
                var thisEntry = arguments[0];

                var lclNum = importer.GrabTemp(thisEntry.Type, thisEntry.ExactSize);
                var asg = new StoreLocalVariableEntry(lclNum, false, thisEntry);
                importer.ImportAppendTree(asg);

                arguments[0] = new LocalVariableEntry(lclNum, thisEntry.Type, thisEntry.ExactSize);

                var node2 = new LocalVariableEntry(lclNum, thisEntry.Type, thisEntry.ExactSize);
                arguments.Add(node2);
            }

            // Intrinsic calls
            if (methodToCall.IsIntrinsic)
            {
                if (ImportIntrinsicCall(methodToCall, arguments, importer))
                {
                    return;
                }
            }
            else
            {
                methodToCall = methodToCall.GetCanonMethodTarget(TypeSystem.Canon.CanonicalFormKind.Specific);
            }

            string targetMethod;
            if (methodToCall.IsPInvoke)
            {
                targetMethod = methodToCall.GetPInvokeMetaData()!.Name;
            }
            else if (methodToCall.IsInternalCall)
            {
                targetMethod = ImportInternalCall(methodToCall);
            }
            else
            {
                targetMethod = importer.NameMangler.GetMangledMethodName(importer.NodeFactory.MethodNode(methodToCall).Method);
            }

            bool directCall = !(opcode == ILOpcode.callvirt && methodToCall.IsVirtual);
            if (methodToCall.HasGenericParameters && !directCall)
            {
                throw new NotSupportedException("Non direct calls to generic methods not supported");
            }

            var returnType = methodToCall.Signature.ReturnType;

            int? returnTypeSize = methodToCall.HasReturnType ? returnType.GetElementSize().AsInt : null;

            var returnVarType = methodToCall.HasReturnType ? returnType.VarType : VarType.Void;
            CallEntry callNode = new CallEntry(targetMethod, arguments, returnVarType, returnTypeSize, !directCall, methodToCall);

            MarkInlineCandidate(callNode, importer);

            if (!methodToCall.HasReturnType)
            {
                importer.ImportAppendTree(callNode, true);
            }
            else
            {
                if (callNode.IsInlineCandidate)
                {
                    // TODO: if struct return then need to spill to a local temp

                    // Split call into two parts: the call itself and the return expression
                    importer.ImportAppendTree(callNode, true);

                    // Link the return expression to the call
                    ReturnExpressionEntry returnExpression = new(callNode);
                    callNode.InlineCandidateInfo!.ReturnExpressionEntry = returnExpression;

                    importer.Push(returnExpression);
                }
                else
                {
                    importer.Push(callNode);
                }
            }
        }

        private static string ImportInternalCall(MethodDesc methodToCall)
        {
            if (!methodToCall.HasCustomAttribute("System.Runtime", "RuntimeImportAttribute"))
            {
                // Simplistic mapping between method and native routine name
                if (IsTypeName(methodToCall, "System", "Console"))
                {
                    var argtype = methodToCall.Parameters[0].Type;
                    return argtype.FullName switch
                    {
                        "System.String" => "PRINT",
                        "System.Int32" => "LTOA",
                        "System.UInt32" => "ULTOA",
                        _ => throw new NotSupportedException("Unknown internal call"),
                    };
                }
            }
            else
            {
                var entryPoint = methodToCall.GetCustomAttributeValue("System.Runtime.RuntimeImportAttribute");
                if (entryPoint == null)
                {
                    throw new NotSupportedException("RuntimeImport missing entrypoint");
                }
                return entryPoint;
            }

            throw new NotSupportedException("Unknown internal call");
        }

        /// <summary>
        /// Try to import a call to an intrinsic method
        /// </summary>
        /// <param name="methodToCall">intrinsic method being called</param>
        /// <param name="arguments">arguments if any</param>
        /// <param name="importer">importer used when generating IR</param>
        /// <returns>true if IR generated to replace call, false otherwise</returns>
        /// <exception cref="NotImplementedException"></exception>
        private static bool ImportIntrinsicCall(MethodDesc methodToCall, List<StackEntry> arguments, IImporter importer)
        {
            // Map method name to string that code generator will understand
            var targetMethodName = methodToCall.Name;
            switch (targetMethodName)
            {
                case "DebugBreak":
                    {
                        var callNode = new IntrinsicEntry(targetMethodName, arguments, VarType.Void);
                        importer.ImportAppendTree(callNode);
                        return true;
                    }

                case "Memmove":
                    if (IsTypeName(methodToCall, "System", "SpanHelpers"))
                    {
                        var size = arguments[0];
                        var sourceAddress = arguments[1];
                        var destinationAddress = arguments[2];
                        var node = new CallEntry("Memcpy", new List<StackEntry> { destinationAddress, sourceAddress, size }, VarType.Void, null);
                        importer.ImportAppendTree(node);
                        return true;
                    }
                    break;

                case "InitializeArray":
                    if (IsTypeName(methodToCall, "System.Runtime.CompilerServices", "RuntimeHelpers"))
                    {
                        return ImportInitializeArray(arguments, importer, methodToCall.Context);
                    }
                    break;

                case "Of":
                    if (IsTypeName(methodToCall, "Internal.Runtime", "EEType"))
                    {
                        var instantiatedType = (InstantiatedMethod)methodToCall;
                        var instantiation = instantiatedType.Instantiation;
                        var objType = instantiation[0];

                        var mangledEETypeName = importer.NameMangler.GetMangledTypeName(objType);

                        importer.Push(new NativeIntConstantEntry(mangledEETypeName));

                        return true;
                    }
                    break;

                case "_Exit":
                    if (IsTypeName(methodToCall, "System", "Environment"))
                    {
                        var callNode = new IntrinsicEntry("Exit", arguments, VarType.Void);
                        importer.ImportAppendTree(callNode);
                        return true;
                    }
                    break;
                case "Write":
                    if (IsTypeName(methodToCall, "System", "Console"))
                    {
                        var argtype = methodToCall.Signature[0].Type;
                        targetMethodName = argtype.FullName switch
                        {
                            "System.Char" => "WriteChar",
                            _ => throw new NotSupportedException(),
                        };
                        var callNode = new IntrinsicEntry(targetMethodName, arguments, VarType.Void);
                        importer.ImportAppendTree(callNode);
                        return true;
                    }
                    break;

                case "get_Chars":
                    {
                        const short StringCharsOffset = 2;

                        var arrayOp = arguments[0];
                        var indexOp = arguments[1];

                        var boundsCheck = !importer.Configuration.SkipArrayBoundsCheck;

                        var node = new IndexRefEntry(indexOp, arrayOp, 2, VarType.UShort, StringCharsOffset, boundsCheck);
                        importer.Push(node);

                        return true;
                    }
            }
            // Treat as normal call
            return false;
        }

        private static bool ImportInitializeArray(IList<StackEntry> arguments, IImporter importer, TypeSystemContext context)
        {
            var arrayRef = arguments[0];
            TokenEntry fieldSlot = (TokenEntry)arguments[1];

            var sourceAddress = new SymbolConstantEntry(fieldSlot.Label);

            var pointerSize = context.Target.PointerSize;

            // Use local to store the array reference as will be used twice
            // Once to get the base size and once to get the array ptr itself

            var arrayRefTemporaryNumber = importer.GrabTemp(arrayRef.Type, pointerSize);
            var arrayRefDefinition = new StoreLocalVariableEntry(arrayRefTemporaryNumber, false, arrayRef);
            importer.ImportAppendTree(arrayRefDefinition);

            arrayRef = new LocalVariableEntry(arrayRefTemporaryNumber, arrayRef.Type, pointerSize);
            var arrayRef2 = new LocalVariableEntry(arrayRefTemporaryNumber, arrayRef.Type, pointerSize);

            // Get the base size
            var eeTypePtr = new IndirectEntry(arrayRef, VarType.Ptr, pointerSize);
            var baseSize = new IndirectEntry(eeTypePtr, VarType.Ptr, pointerSize, 4);   // Base size is at offset 4

            StackEntry destinationAddress = new BinaryOperator(Operation.Add, isComparison: false, arrayRef2, baseSize, VarType.Ptr);

            var arrayData = ((DnlibField)fieldSlot.Field).GetFieldRvaData();
            var size = new NativeIntConstantEntry((short)arrayData.Length);

            var args = new List<StackEntry>() { size, sourceAddress, destinationAddress };

            var node = new CallEntry("Memcpy", args, VarType.Void, null);
            importer.ImportAppendTree(node);

            return true;
        }

        private static bool IsTypeName(MethodDesc method, string typeNamespace, string typeName)
        {
            var metadataType = method.OwningType;
            if (metadataType == null)
            {
                return false;
            }
            return metadataType.Namespace == typeNamespace && metadataType.Name == typeName;
        }

        private static IndirectEntry DereferenceThisPtr(StackEntry thisPtr, TypeDesc thisType)
        {
            return new IndirectEntry(thisPtr, thisType.VarType, thisType.GetElementSize().AsInt);
        }

        private static StackEntry NormalizeStructValue(StackEntry argument, IImporter importer)
        {
            if (argument is CallEntry)
            {
                var temp = importer.GrabTemp(argument.Type, argument.ExactSize);

                // Import store to temp
                StackEntry store = importer.NewTempStore(temp, argument);
                importer.ImportAppendTree(store);

                argument = new LocalVariableEntry(temp, argument.Type, argument.ExactSize);
            }

            return argument;
        }

        private static void MarkInlineCandidate(CallEntry call, IImporter importer)
        {
            InlineContext inlinersContext;
            if (importer.InlineInfo is not null)
            {
                inlinersContext = importer.InlineInfo.InlineContext!;
            }
            else
            {
                // Use root context
                inlinersContext = RootContext;
            }

            var inlineResult = new InlineResult() { InlineCall = call };

            if (!importer.Configuration.Optimize)
            {
                inlineResult.NoteFatal(InlineObservation.DebugCodeGen);
                return;
            }

            if (call.Method!.IsPInvoke)
            {
                inlineResult.NoteFatal(InlineObservation.IsPInvoke);
                return;
            }

            if (call.Method!.IsInternalCall)
            {
                inlineResult.NoteFatal(InlineObservation.IsInternal);
                return;
            }

            if (call.Method!.IsVirtual)
            {
                inlineResult.NoteFatal(InlineObservation.IsVirtual);
                return;
            }

            if (call.Method.IsNoInlining)
            {
                inlineResult.NoteFatal(InlineObservation.IsNoInline);
                return;
            }

            if (call.Method.Signature.ReturnType.VarType == VarType.Struct)
            {
                inlineResult.NoteFatal(InlineObservation.IsStructReturn);
                return;
            }

            // Temporarily only inline methods marked with AggressiveInlining
            // TODO: need to investigate more aggressive inlining as currently
            // results in too much code bloat
            if (!call.Method.IsAggressiveInlining)
            {
                inlineResult.NoteFatal(InlineObservation.NotMarkedForAggressiveInlining);
                return;
            }

            var inlineCandidateInfo = CheckCanInline(call, inlinersContext, inlineResult);

            if (inlineResult.IsFailure)
            {
                return;
            }

            if (call.Method!.MethodIL!.GetExceptionRegions().Length > 0)
            {
                // TODO: check that inline location is within a filter
                // See -> https://github.com/dotnet/runtime/commit/dc88476f102123edebd6b2d2efe5a56146f60094#diff-266282d463447c6984d3223b87f05a0cc94197441c10c204a8f89af0d3d8a372
                // if (bbInFilterBBRange(compCurBB) {
                inlineResult.NoteFatal(InlineObservation.IsWithinFilter);
                // }
                return;
            }

            call.InlineCandidateInfo = inlineCandidateInfo;

            // Method is a candidate for inlining
            call.IsInlineCandidate = true;
        }

        private static InlineCandidateInfo? CheckCanInline(CallEntry call, InlineContext inlinersContext, InlineResult inlineResult)
        {
            if (call.Method?.MethodIL is null)
            {
                inlineResult.NoteFatal(InlineObservation.NoMethodInfo);
                return null;
            }

            CanInlineIL(call.Method, inlineResult);

            var inlineCandidateInfo = new InlineCandidateInfo();
            inlineCandidateInfo.InlinersContext = inlinersContext;
            return inlineCandidateInfo;
        }

        private static void CanInlineIL(MethodDesc methodInfo, InlineResult inlineResult)
        {
            if (methodInfo.MethodIL?.Instructions == null || methodInfo.MethodIL.Instructions.Count == 0)
            {
                inlineResult.NoteFatal(InlineObservation.HasNoBody);
                return;
            }

            inlineResult.NoteBool(InlineObservation.IsForceInline, methodInfo.IsAggressiveInlining);

            inlineResult.NoteInt(InlineObservation.ILCodeSize, methodInfo.MethodIL.ILCodeSize);
        }

        private static InlineContext RootContext { get; } = new InlineContext();
    }
}
