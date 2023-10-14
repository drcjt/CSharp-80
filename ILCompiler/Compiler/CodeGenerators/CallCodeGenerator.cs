using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class CallCodeGenerator : ICodeGenerator<CallEntry>
    {
        public void GenerateCode(CallEntry entry, CodeGeneratorContext context)
        {
            if (entry.IsVirtual)
            {
                GenerateCodeForVirtualCall(entry, context);
            }
            else
            {
                GenerateCodeForCall(entry, context);
            }
        }

        private static void GenerateCodeForCall(CallEntry entry, CodeGeneratorContext context)
        {
            if (entry.Method != null && entry.Method.IsInternalCall && entry.Arguments.Count > 0)
            {
                // Pass last argument in HL for Ptr type
                // and in HL, DE otherwise
                var argument = entry.Arguments[entry.Arguments.Count - 1];
                context.InstructionsBuilder.Pop(HL);

                if (argument.Type != VarType.Ptr && argument.Type != VarType.Ref) // TODO: should this also check for ByRef??
                {
                    context.InstructionsBuilder.Pop(DE);
                }
            }
            context.InstructionsBuilder.Call(entry.TargetMethod);
        }

        private static void GenerateCodeForVirtualCall(CallEntry entry, CodeGeneratorContext context)
        {
            var targetMethod = entry.Method;

            if (targetMethod == null)
            {
                throw new InvalidOperationException("Virtual CallEntry must have non-null Method");
            }

            int slot = VirtualMethodSlotHelper.GetVirtualMethodSlot(context.NodeFactory, targetMethod, targetMethod.DeclaringType);

            // EEType header comprises of following:
            //    2 bytes for Flags
            //    2 bytes for base size
            //    2 bytes for related type
            const int eeTypeHeader = 3 * 2;
            context.InstructionsBuilder.Ld(BC, (byte)((slot * 2) + eeTypeHeader));

            context.InstructionsBuilder.Call("VirtualCall");
        }
    }
}
