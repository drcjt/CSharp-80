using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public class CallImporter : IOpcodeImporter
    {
        public bool CanImport(Code code) => code == Code.Call;

        public void Import(Instruction instruction, ImportContext context, IILImporterProxy importer)
        {
            ImportCall(instruction, context, importer);
        }

        public static void ImportCall(Instruction instruction, ImportContext context, IILImporterProxy importer, StackEntry? newObjThis = null)
        {
            var methodDefOrRef = instruction.Operand as IMethodDefOrRef;
            var methodToCall = methodDefOrRef.ResolveMethodDefThrow();

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
            if (methodToCall.IsIntrinsic())
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

            var callNode = new CallEntry(targetMethod, arguments, returnType.GetStackValueKind(), returnTypeSize);
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
                    var loadTemp = new LocalVariableEntry(returnBufferArgIndex, returnType.GetStackValueKind(), returnType.GetExactSize());
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
            var lclNum = importer.GrabTemp(returnType.GetStackValueKind(), returnType.GetExactSize(), returnType.GetVarType());
            var returnBufferPtr = new LocalVariableAddressEntry(lclNum);

            // Ensure return buffer parameter goes after the this parameter if present
            var returnBufferArgPos = hasThis ? 1 : 0;
            arguments.Insert(returnBufferArgPos, returnBufferPtr);

            return lclNum;
        }

        private static bool ImportIntrinsicCall(MethodDef methodToCall, IList<StackEntry> arguments, IILImporterProxy importer)
        {
            // Not yet implemented methods with non void return type
            if (methodToCall.HasReturnType)
            {
                throw new NotSupportedException();
            }

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
                default:
                    return false;
            }

            var callNode = new IntrinsicEntry(targetMethodName, arguments, StackValueKind.Unknown);
            importer.ImportAppendTree(callNode);

            return true;
        }

        private static bool IsTypeName(MethodDef method, string typeNamespace, string typeName)
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