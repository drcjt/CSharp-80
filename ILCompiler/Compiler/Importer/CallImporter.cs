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
        public bool CanImport(Code code) => code == Code.Call;

        public void Import(Instruction instruction, ImportContext context, IILImporter importer)
        {
            var methodDefOrRef = instruction.Operand as IMethodDefOrRef;
            var methodToCall = methodDefOrRef.ResolveMethodDefThrow();

            var arguments = new StackEntry[methodToCall.Parameters.Count];
            for (var i = 0; i < methodToCall.Parameters.Count; i++)
            {
                var argument = importer.PopExpression();
                arguments[methodToCall.Parameters.Count - i - 1] = argument;
            }

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

            var returnType = methodToCall.ReturnType.GetStackValueKind();
            if (returnType == StackValueKind.ValueType)
            {
                // TODO: valuetype does not imply struct
                // need to deal with enums here too

                // Return type is a struct
                // generate new temp to act as return buffer
                // need to add extra hidden parameter to call that will be a pointer to temp to
                // hold returned struct
                // Also need to add exta node after call to load temp to stack
            }

            var callNode = new CallEntry(targetMethod, arguments, returnType);

            if (!methodToCall.HasReturnType)
            {
                importer.ImportAppendTree(callNode);
            }
            else
            {
                importer.PushExpression(callNode);
            }
        }

        private static bool ImportIntrinsicCall(MethodDef methodToCall, StackEntry[] arguments, IILImporter importer)
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