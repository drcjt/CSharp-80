using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class CallImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            switch (instruction.OpCode.Code)
            {
                case Code.Call:
                case Code.Callvirt:
                    ImportCall(instruction, context, importer);
                    return true;

                default:
                    return false;
            }
        }

        public static void ImportCall(Instruction instruction, ImportContext context, IILImporterProxy importer, StackEntry? newObjThis = null)
        {
            if (instruction.Operand is not IMethod method)
            {
                throw new InvalidOperationException("Newobj importer called with Operand which isn't a IMethod");
            }

            var isArrayMethod = false;
            MethodDef methodToCall;
            if (method.DeclaringType.ToTypeSig().IsArray)
            {
                var declaringTypeDef = method.DeclaringType.ResolveTypeDef();
                var methodName = method.Name;
                switch (methodName)
                {
                    case "Set":
                        methodToCall = new MethodDefUser("Set", method.MethodSig);
                        declaringTypeDef.Methods.Add(methodToCall);
                        break;
                    case "Get":
                        methodToCall = new MethodDefUser("Get", method.MethodSig);
                        declaringTypeDef.Methods.Add(methodToCall);
                        break;
                    case "Address":
                        methodToCall = new MethodDefUser("Address", method.MethodSig);
                        declaringTypeDef.Methods.Add(methodToCall);
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown array intrinsic method {methodName}");
                }
                isArrayMethod = true;
            }
            else
            {
                methodToCall = method.ResolveMethodDefThrow();
            }
            
            var arguments = new List<StackEntry>();
            var firstArgIndex = newObjThis != null ? 1 : 0;
            var parameterCount = methodToCall.Parameters.Count;
            for (var i = firstArgIndex; i < parameterCount; i++)
            {
                var argument = importer.PopExpression();

                var parameter = methodToCall.Parameters[parameterCount - (i - firstArgIndex) - 1];
                var parameterType = parameter.Type;

                parameterType = GenericTypeInstantiator.Instantiate(parameterType, method, context.Method);
                var parameterVarType = parameterType.GetVarType();
                if (parameterVarType.IsSmall())
                {
                    argument = new PutArgTypeEntry(parameterVarType, argument);
                }

                arguments.Add(argument);
            }
            // Add the this pointer if required, e.g. if part of newobj
            if (newObjThis != null)
            {
                arguments.Add(newObjThis);
            }
            arguments.Reverse();

            if (instruction.OpCode == OpCodes.Callvirt)
            {
                // Turn first argument into throw null ref check
                arguments[0] = new NullCheckEntry(arguments[0]);
            }

            if (methodToCall.DeclaringType.IsInterface)
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
            if (methodToCall.IsIntrinsic() || isArrayMethod)
            {
                if (ImportIntrinsicCall(methodToCall, arguments, importer, method, context))
                {
                    return;
                }
            }

            string targetMethod;
            if (methodToCall.HasGenericParameters)
            {
                targetMethod = context.NameMangler.GetMangledMethodName((MethodSpec)method, context.Method);
            }
            else if (methodToCall.IsPinvokeImpl)
            {
                targetMethod = methodToCall.ImplMap.Name;
            }
            else if (methodToCall.IsInternalCall)
            {
                targetMethod = ImportInternalCall(methodToCall, arguments, importer);
            }
            else
            {
                targetMethod = context.NameMangler.GetMangledMethodName(methodToCall);
            }

            bool directCall = !(instruction.OpCode.Code == Code.Callvirt && methodToCall.IsVirtual);
            if (methodToCall.HasGenericParameters && !directCall)
            {
                throw new NotSupportedException("Non direct calls to generic methods not supported");
            }

            int returnBufferArgIndex = 0;
            var returnType = GenericTypeInstantiator.Instantiate(methodToCall.ReturnType, method, context.Method);
            if (methodToCall.HasReturnType)
            {
                if (returnType.IsStruct())
                {
                    returnBufferArgIndex = FixupCallStructReturn(returnType, arguments, importer, methodToCall.HasThis);
                }
            }

            int? returnTypeSize = methodToCall.HasReturnType ? returnType.GetInstanceFieldSize() : null;

            var returnVarType = methodToCall.HasReturnType ? returnType.GetVarType() : VarType.Void;
            StackEntry callNode = new CallEntry(targetMethod, arguments, returnVarType, returnTypeSize, !directCall, methodToCall);

            if (methodToCall.IsStatic && !context.PreinitializationManager.IsPreinitialized(methodToCall.DeclaringType))
            {
                callNode = InitClassHelper.ImportInitClass(methodToCall, context, importer, callNode);
            }

            if (!methodToCall.HasReturnType)
            {
                importer.ImportAppendTree(callNode, true);
            }
            else
            {
                if (returnType.IsStruct())
                {
                    importer.ImportAppendTree(callNode, true);

                    // Load return buffer to stack
                    var loadTemp = new LocalVariableEntry(returnBufferArgIndex, returnType.GetVarType(), returnType.GetInstanceFieldSize());
                    importer.PushExpression(loadTemp);
                }
                else
                {
                    importer.PushExpression(callNode);
                }
            }
        }

        static private int FixupCallStructReturn(TypeSig returnType, List<StackEntry> arguments, IILImporterProxy importer, bool hasThis)
        {
            // Create temp
            var lclNum = importer.GrabTemp(returnType.GetVarType(), returnType.GetInstanceFieldSize());
            var returnBufferPtr = new LocalVariableAddressEntry(lclNum);

            // Ensure return buffer parameter goes after the this parameter if present
            var returnBufferArgPos = hasThis ? 1 : 0;
            arguments.Insert(returnBufferArgPos, returnBufferPtr);

            return lclNum;
        }

        private static string ImportInternalCall(MethodDef methodToCall, IList<StackEntry> arguments, IILImporterProxy importer)
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
                throw new NotSupportedException("Unknown internal call");
            }
            else
            {
                if (methodToCall.CustomAttributes[0].ConstructorArguments[0].Value is not UTF8String entryPoint)
                {
                    throw new NotSupportedException("RuntimeImport missing entrypoint");
                }
                return entryPoint.String;
            }
        }

        /// <summary>
        /// Try to import a call to an intrinsic method
        /// </summary>
        /// <param name="methodToCall">intrinsic method being called</param>
        /// <param name="arguments">arguments if any</param>
        /// <param name="importer">importer used when generating IR</param>
        /// <returns>true if IR generated to replace call, false otherwise</returns>
        /// <exception cref="NotImplementedException"></exception>
        private static bool ImportIntrinsicCall(MethodDef methodToCall, IList<StackEntry> arguments, IILImporterProxy importer, IMethod method, ImportContext context)
        {
            // Map method name to string that code generator will understand
            var targetMethodName = methodToCall.Name;
            switch (targetMethodName)
            {
                case "Of":
                    if (IsTypeName(methodToCall, "Internal.Runtime", "EEType"))
                    {
                        var genericParameters = ((MethodSpec)method).GenericInstMethodSig.GenericArguments;
                        var objType = GenericTypeInstantiator.Instantiate(genericParameters[0], method, context.Method);
                        var typeDef = objType.ToTypeDefOrRef().ResolveTypeDef();

                        var mangledEETypeName = context.NameMangler.GetMangledTypeName(typeDef);

                        importer.PushExpression(new NativeIntConstantEntry(mangledEETypeName));

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
                        var argtype = methodToCall.Parameters[0].Type;
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
                case "Get":
                    {
                        // first parameter = this, remaining parameters = indices
                        var valueType = methodToCall.Parameters[1].Type;
                        var elemSize = valueType.GetInstanceFieldSize();
                        var rank = methodToCall.Parameters.Count - 1;

                        // Arrays are stored in row-major order see https://github.com/stakx/ecma-335/blob/master/docs/i.8.9.1-array-types.md

                        // Calculation should follow details here https://en.wikipedia.org/wiki/Row-_and_column-major_order
                        // The calculation below is incorrect

                        // Calculate indices multipled together
                        StackEntry indexOp = new CastEntry(arguments[1], VarType.Ptr);
                        for (var dimension = 1; dimension < rank; dimension++)
                        {
                            var nextIndexOp = new CastEntry(arguments[dimension + 1], VarType.Ptr);
                            indexOp = new BinaryOperator(Operation.Mul, isComparison: false, indexOp, nextIndexOp, VarType.Ptr);
                        }
                        // Multiply by elemSize
                        var size = new NativeIntConstantEntry((short)elemSize);
                        indexOp = new BinaryOperator(Operation.Mul, isComparison: false, indexOp, size, VarType.Ptr);

                        // Add address of array
                        var addr = new BinaryOperator(Operation.Add, isComparison: false, indexOp, arguments[0], VarType.Ptr);

                        var op = new IndirectEntry(addr, valueType.GetVarType(), elemSize);
                        importer.PushExpression(op);

                        return true;
                    }
                case "Set":
                    {
                        // first parameter = this, second parameter = value, remaining parameters = indices
                        var valueType = methodToCall.Parameters[1].Type;
                        var elemSize = valueType.GetInstanceFieldSize();
                        var rank = methodToCall.Parameters.Count - 2;

                        // Arrays are stored in row-major order see https://github.com/stakx/ecma-335/blob/master/docs/i.8.9.1-array-types.md

                        // Calculation should follow details here https://en.wikipedia.org/wiki/Row-_and_column-major_order
                        // The calcualtion below is incorrect

                        // Calculate indices multipled together
                        StackEntry indexOp = new CastEntry(arguments[1], VarType.Ptr);
                        for (var dimension = 1; dimension < rank; dimension++)
                        {
                            var nextIndexOp = new CastEntry(arguments[dimension + 1], VarType.Ptr);
                            indexOp = new BinaryOperator(Operation.Mul, isComparison: false, indexOp, nextIndexOp, VarType.Ptr);
                        }
                        // Multiply by elemSize
                        var size = new NativeIntConstantEntry((short)elemSize);
                        indexOp = new BinaryOperator(Operation.Mul, isComparison: false, indexOp, size, VarType.Ptr);

                        // Add address of array
                        var addr = new BinaryOperator(Operation.Add, isComparison: false, indexOp, arguments[0], VarType.Ptr);

                        var op = new StoreIndEntry(addr, arguments[rank + 1], valueType.GetVarType(), 0, elemSize);
                        importer.ImportAppendTree(op);

                        return true;
                    }
                case "Address":
                    throw new NotImplementedException("Multidimensional arrays not supported");

                case "get_Chars":
                    {
                        const short StringCharsOffset = 2;

                        var arrayOp = arguments[0];
                        var indexOp = arguments[1];

                        var boundsCheck = !context.Configuration.SkipArrayBoundsCheck;

                        var node = new IndexRefEntry(indexOp, arrayOp, 2, VarType.UShort, StringCharsOffset, boundsCheck);
                        importer.PushExpression(node);

                        return true;
                    }
            }
            // Treat as normal call
            return false;
        }

        private static bool IsTypeName(IMethodDefOrRef method, string typeNamespace, string typeName)
        {
            var metadataType = method.DeclaringType;
            if (metadataType == null)
            {
                return false;
            }
            return metadataType.Namespace == typeNamespace && metadataType.Name == typeName;
        }
    }
}