﻿using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.Dnlib;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler.Importer
{
    public class CallImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            switch (instruction.Opcode)
            {
                case ILOpcode.call:
                case ILOpcode.callvirt:
                    ImportCall(instruction, context, importer);
                    return true;

                default:
                    return false;
            }
        }

        public static void ImportCall(Instruction instruction, ImportContext context, IILImporterProxy importer, StackEntry? newObjThis = null)
        {
            var method = (MethodDesc)instruction.Operand;
            ImportCall(method, instruction, context, importer, newObjThis);
        }

        public static void ImportCall(MethodDesc method, Instruction instruction, ImportContext context, IILImporterProxy importer, StackEntry? newObjThis = null)
        {
            var isArrayMethod = false;
            MethodDesc methodToCall;
            var opcode = instruction.Opcode;
            if (method.OwningType.IsSzArray)
            {
                switch (method.Name)
                {
                    case "Set":
                        methodToCall = method.CreateUserMethod("Set");
                        break;
                    case "Get":
                        methodToCall = method.CreateUserMethod("Get");
                        break;
                    case "Address":
                        methodToCall = method.CreateUserMethod("Address");
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown array intrinsic method {method.Name}");
                }
                isArrayMethod = true;
            }
            else
            {
                methodToCall = method;
            }

            var arguments = new List<StackEntry>();
            for (var i = methodToCall.Signature.Length - 1; i >= 0; i--)
            {
                var argument = importer.PopExpression();

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

            // Add the this pointer if required, e.g. if part of newobj
            if (newObjThis != null)
            {
                arguments.Add(newObjThis);
            }
            else if (methodToCall.HasThis)
            {
                var thisPtr = importer.PopExpression();
                arguments.Add(thisPtr);
            }
            arguments.Reverse();


            if (opcode == ILOpcode.callvirt)
            {
                if (context.Constrained != null)
                {
                    TypeDesc constrained = context.Constrained;
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
                        var boxedNode = BoxImporter.BoxValue(dereferencedThisPtr, constrainedType, context, importer);
                        arguments[0] = boxedNode;
                    }
                    else
                    {
                        // Dereference this ptr
                        arguments[0] = DereferenceThisPtr(arguments[0], constrainedType);
                    }

                    context.Constrained = null;
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
            if (methodToCall.IsIntrinsic || isArrayMethod)
            {
                if (ImportIntrinsicCall(methodToCall, arguments, importer, method, context))
                {
                    return;
                }
            }

            if (!methodToCall.IsIntrinsic)
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
                targetMethod = ImportInternalCall(methodToCall, arguments, importer);
            }
            else
            {
                targetMethod = context.NameMangler.GetMangledMethodName(methodToCall);
            }

            bool directCall = !(opcode == ILOpcode.callvirt && methodToCall.IsVirtual);
            if (methodToCall.HasGenericParameters && !directCall)
            {
                throw new NotSupportedException("Non direct calls to generic methods not supported");
            }

            int returnBufferArgIndex = 0;
            var returnType = methodToCall.Signature.ReturnType;
            if (methodToCall.HasReturnType)
            {
                if (returnType.IsValueType && !returnType.IsPrimitive && !returnType.IsEnum)
                {
                    returnBufferArgIndex = FixupCallStructReturn(returnType, arguments, importer, methodToCall.HasThis);
                }
            }

            int? returnTypeSize = methodToCall.HasReturnType ? returnType.GetElementSize().AsInt : null;

            var returnVarType = methodToCall.HasReturnType ? returnType.VarType : VarType.Void;
            StackEntry callNode = new CallEntry(targetMethod, arguments, returnVarType, returnTypeSize, !directCall, methodToCall);

            if (methodToCall.IsStatic && !context.PreinitializationManager.IsPreinitialized(methodToCall.OwningType))
            {
                callNode = InitClassHelper.ImportInitClass(methodToCall.OwningType, context, importer, callNode);
            }

            if (!methodToCall.HasReturnType)
            {
                importer.ImportAppendTree(callNode, true);
            }
            else
            {
                if (returnType.IsValueType && !returnType.IsPrimitive && !returnType.IsEnum)
                {
                    importer.ImportAppendTree(callNode, true);

                    // Load return buffer to stack
                    var loadTemp = new LocalVariableEntry(returnBufferArgIndex, returnType.VarType, returnType.GetElementSize().AsInt);
                    importer.PushExpression(loadTemp);
                }
                else
                {
                    importer.PushExpression(callNode);
                }
            }
        }

        static private int FixupCallStructReturn(TypeDesc returnType, List<StackEntry> arguments, IILImporterProxy importer, bool hasThis)
        {
            // Create temp
            var lclNum = importer.GrabTemp(returnType.VarType, returnType.GetElementSize().AsInt);
            var returnBufferPtr = new LocalVariableAddressEntry(lclNum);

            // Ensure return buffer parameter goes after the this parameter if present
            var returnBufferArgPos = hasThis ? 1 : 0;
            arguments.Insert(returnBufferArgPos, returnBufferPtr);

            return lclNum;
        }

        private static string ImportInternalCall(MethodDesc methodToCall, IList<StackEntry> arguments, IILImporterProxy importer)
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
        private static bool ImportIntrinsicCall(MethodDesc methodToCall, IList<StackEntry> arguments, IILImporterProxy importer, MethodDesc method, ImportContext context)
        {
            // Map method name to string that code generator will understand
            var targetMethodName = methodToCall.Name;
            switch (targetMethodName)
            {
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

                        var mangledEETypeName = context.NameMangler.GetMangledTypeName(objType);

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
                case "Get":
                    {
                        // first parameter = this, remaining parameters = indices
                        var valueType = methodToCall.Signature[0];
                        var elemSize = valueType.Type.GetElementSize().AsInt;
                        var rank = methodToCall.Signature.Length;

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

                        var op = new IndirectEntry(addr, valueType.Type.VarType, elemSize);
                        importer.PushExpression(op);

                        return true;
                    }
                case "Set":
                    {
                        // first parameter = this, second parameter = value, remaining parameters = indices
                        var valueType = methodToCall.Signature[0];
                        var elemSize = valueType.Type.GetElementSize().AsInt;
                        var rank = methodToCall.Signature.Length - 1;

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

                        var op = new StoreIndEntry(addr, arguments[rank + 1], valueType.Type.VarType, 0, elemSize);
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

        private static bool ImportInitializeArray(IList<StackEntry> arguments, IILImporterProxy importer, TypeSystemContext context)
        {
            var arrayObjPtr = arguments[0];
            TokenEntry fieldSlot = (TokenEntry)arguments[1];

            var sourceAddress = new SymbolConstantEntry(fieldSlot.Label);

            int pointerSize = context.Target.PointerSize;
            var arrayBaseSize = new NativeIntConstantEntry((short)(pointerSize + sizeof(ushort)));    // EEType Ptr and array length
            StackEntry destinationAddress = new BinaryOperator(Operation.Add, isComparison: false, arrayObjPtr, arrayBaseSize, VarType.Ptr);

            var arrayData = ((DnlibField)fieldSlot.Field).GetFieldRvaData();
            var size = new Int32ConstantEntry((short)arrayData.Length);

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

        private static StackEntry DereferenceThisPtr(StackEntry thisPtr, TypeDesc thisType)
        {
            return new IndirectEntry(thisPtr, thisType.VarType, thisType.GetElementSize().AsInt);
        }
    }
}