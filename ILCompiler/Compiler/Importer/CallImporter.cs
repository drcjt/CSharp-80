using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class CallImporter : IOpcodeImporter
    {
        public bool Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            if (instruction.OpCode.Code != Code.Call) return false;

            ImportCall(instruction, context, importer);
            return true;
        }

        public static void ImportCall(Instruction instruction, ImportContext context, IILImporterProxy importer, StackEntry? newObjThis = null)
        {
            var methodDefOrRef = instruction.Operand as IMethodDefOrRef;

            if (methodDefOrRef == null)
            {
                throw new InvalidOperationException("Newobj importer called with Operand which isn't a IMethodDefOrRef");
            }

            var isArrayMethod = false;
            MethodDef methodToCall;
            if (methodDefOrRef.DeclaringType.ToTypeSig().IsArray)
            {
                var declaringTypeDef = methodDefOrRef.DeclaringType.ResolveTypeDef();
                var methodName = methodDefOrRef.Name;
                switch (methodName)
                {
                    case "Set":
                        methodToCall = new MethodDefUser("Set", methodDefOrRef.MethodSig);
                        declaringTypeDef.Methods.Add(methodToCall);
                        break;
                    case "Get":
                        methodToCall = new MethodDefUser("Get", methodDefOrRef.MethodSig);
                        declaringTypeDef.Methods.Add(methodToCall);
                        break;
                    case "Address":
                        methodToCall = new MethodDefUser("Address", methodDefOrRef.MethodSig);
                        declaringTypeDef.Methods.Add(methodToCall);
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown array intrinsic method {methodName}");
                }
                isArrayMethod = true;
            }
            else
            {
                methodToCall = methodDefOrRef.ResolveMethodDefThrow();
            }

            var arguments = new List<StackEntry>();
            var firstArgIndex = newObjThis != null ? 1 : 0;
            var parameterCount = methodToCall.Parameters.Count;
            for (var i = firstArgIndex; i < parameterCount; i++)
            {
                var argument = importer.PopExpression();

                var parameterVarType = methodToCall.Parameters[parameterCount - (i - firstArgIndex) - 1].Type.GetVarType();
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

            // Intrinsic calls
            if (methodToCall.IsIntrinsic() || isArrayMethod)
            {
                if (!ImportIntrinsicCall(methodToCall, arguments, importer))
                {
                    throw new NotSupportedException("Unknown intrinsic");
                }
                return;
            }

            string targetMethod;
            if (methodToCall.IsPinvokeImpl)
            {
                targetMethod = methodToCall.ImplMap.Name;
            }
            else
            {
                targetMethod = context.NameMangler.GetMangledMethodName(methodToCall);
            }

            int returnBufferArgIndex = 0;
            var returnType = methodToCall.ReturnType;
            if (methodToCall.HasReturnType)
            {
                if (returnType.IsStruct())
                {
                    returnBufferArgIndex = FixupCallStructReturn(returnType, arguments, importer, methodToCall.HasThis);
                }
            }

            int? returnTypeSize = methodToCall.HasReturnType ? returnType.GetExactSize() : null;

            var callNode = new CallEntry(targetMethod, arguments, returnType.GetVarType(), returnTypeSize);
            callNode.Type = methodToCall.HasReturnType ? returnType.GetVarType() : VarType.Void;

            if (!methodToCall.HasReturnType)
            {
                importer.ImportAppendTree(callNode);
            }
            else
            {
                if (returnType.IsStruct())
                {
                    importer.ImportAppendTree(callNode);

                    // Load return buffer to stack
                    var loadTemp = new LocalVariableEntry(returnBufferArgIndex, returnType.GetVarType(), returnType.GetExactSize());
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
            var lclNum = importer.GrabTemp(returnType.GetVarType(), returnType.GetExactSize());
            var returnBufferPtr = new LocalVariableAddressEntry(lclNum);

            // Ensure return buffer parameter goes after the this parameter if present
            var returnBufferArgPos = hasThis ? 1 : 0;
            arguments.Insert(returnBufferArgPos, returnBufferPtr);

            return lclNum;
        }

        private static bool ImportIntrinsicCall(MethodDef methodToCall, IList<StackEntry> arguments, IILImporterProxy importer)
        {
            // Map method name to string that code generator will understand
            var targetMethodName = "";
            switch (methodToCall.Name)
            {
                // TODO: Suspect this won't stay as an intrinsic but at least we have the mechanism for instrincs
                case "Write":
                    if (IsTypeName(methodToCall, "System", "Console"))
                    {
                        var argtype = methodToCall.Parameters[0].Type;
                        targetMethodName = argtype.FullName switch
                        {
                            "System.String" => "WriteString",
                            "System.Int32" => "WriteInt32",
                            "System.UInt32" => "WriteUInt32",
                            "System.Char" => "WriteChar",
                            _ => throw new NotSupportedException(),
                        };
                    }
                    break;

                case "Get":
                    {
                        // first parameter = this, remaining parameters = indices
                        var valueType = methodToCall.Parameters[1].Type;
                        var elemSize = valueType.GetExactSize();
                        var rank = methodToCall.Parameters.Count - 1;

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

                        var op = new IndirectEntry(addr, valueType.GetVarType(), elemSize);
                        importer.PushExpression(op);

                        return true;
                    }
                case "Set":
                    {
                        // first parameter = this, second parameter = value, remaining parameters = indices
                        var valueType = methodToCall.Parameters[1].Type;
                        var elemSize = valueType.GetExactSize();
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

                        var op = new StoreIndEntry(addr, arguments[rank + 1], 0, elemSize);
                        importer.ImportAppendTree(op);

                        return true;
                    }
                case "Address":
                    throw new NotImplementedException("Multidimensional arrays not supported");
                default:
                    return false;
            }

            var callNode = new IntrinsicEntry(targetMethodName, arguments, VarType.Void);
            importer.ImportAppendTree(callNode);

            return true;
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