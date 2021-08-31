using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System;

namespace ILCompiler.Compiler.Importer
{
    public class CallImporter : IOpcodeImporter
    {
        private readonly IILImporter _importer;
        public CallImporter(IILImporter importer)
        {
            _importer = importer;
        }

        public bool CanImport(Code opcode)
        {
            return opcode == Code.Call;
        }

        public void Import(Instruction instruction, ImportContext context)
        {
            var methodDefOrRef = instruction.Operand as IMethodDefOrRef;
            var methodToCall = methodDefOrRef.ResolveMethodDefThrow();

            StackEntry[] arguments = new StackEntry[methodToCall.Parameters.Count];
            for (int i = 0; i < methodToCall.Parameters.Count; i++)
            {
                var argument = _importer.PopExpression();
                arguments[methodToCall.Parameters.Count - i - 1] = argument;
            }

            // Intrinsic calls
            if (methodToCall.IsIntrinsic())
            {
                if (!ImportIntrinsicCall(methodToCall, arguments))
                {
                    throw new NotSupportedException("Unknown intrinsic");
                }
                return;
            }

            string targetMethod = "";
            if (methodToCall.IsPinvokeImpl)
            {
                targetMethod = methodToCall.ImplMap.Name;
            }
            else
            {
                targetMethod = context.NameMangler.GetMangledMethodName(methodToCall);
            }
            var returnType = methodToCall.ReturnType.GetStackValueKind();
            var callNode = new CallEntry(targetMethod, arguments, returnType);
            if (!methodToCall.HasReturnType)
            {
                _importer.ImportAppendTree(callNode);
            }
            else
            {
                _importer.PushExpression(callNode);
            }
        }

        private bool ImportIntrinsicCall(MethodDef methodToCall, StackEntry[] arguments)
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
                        switch (argtype.FullName)
                        {
                            case "System.String":
                                targetMethodName = "WriteString";
                                break;

                            case "System.Int32":
                                targetMethodName = "WriteInt32";
                                break;

                            case "System.UInt32":
                                targetMethodName = "WriteUInt32";
                                break;

                            case "System.Char":
                                targetMethodName = "WriteChar";
                                break;

                            default:
                                throw new NotSupportedException();
                        }
                    }
                    break;
                default:
                    return false;
            }

            var callNode = new IntrinsicEntry(targetMethodName, arguments, StackValueKind.Unknown);
            _importer.ImportAppendTree(callNode);

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