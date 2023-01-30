using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;

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
            var method = instruction.Operand as IMethod;

            if (method == null)
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
                if (parameter.Type.IsGenericMethodParameter)
                {
                    var methodSpec = (MethodSpec)method;
                    parameterType = GenericTypeInstantiator.Instantiate(parameterType, methodSpec.GenericInstMethodSig.GenericArguments);
                }

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
            if (methodToCall.HasGenericParameters)
            {
                var methodSpec = (MethodSpec)method;
                targetMethod = context.NameMangler.GetMangledMethodName(methodSpec);
            }
            else if (methodToCall.IsPinvokeImpl)
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

            int? returnTypeSize = methodToCall.HasReturnType ? returnType.GetInstanceFieldSize() : null;

            var returnVarType = methodToCall.HasReturnType ? returnType.GetVarType() : VarType.Void;
            var callNode = new CallEntry(targetMethod, arguments, returnVarType, returnTypeSize);

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
                        var elemSize = valueType.GetInstanceFieldSize();
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
                        var arrayOp = arguments[0];
                        var indexOp = arguments[1];

                        var node = new IndexRefEntry(indexOp, arrayOp, 2, VarType.UShort);
                        importer.PushExpression(node);

                        return true;
                    }

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