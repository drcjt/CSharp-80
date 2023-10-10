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
            if (entry.IsInternalCall && entry.Arguments.Count > 0)
            {
                // Pass last argument in HL for Ptr type
                // and in HL, DE otherwise
                var argument = entry.Arguments[entry.Arguments.Count - 1];
                context.Emitter.Pop(HL);

                if (argument.Type != VarType.Ptr && argument.Type != VarType.Ref) // TODO: should this also check for ByRef??
                {
                    context.Emitter.Pop(DE);
                }
            }
            context.Emitter.Call(entry.TargetMethod);
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

            // Get this pointer into HL
            context.Emitter.Pop(HL);
            context.Emitter.Push(HL);

            // Get address of EEType into HL from first 2 btytes of object
            context.Emitter.Ld(E, __[HL]);
            context.Emitter.Inc(HL);
            context.Emitter.Ld(D, __[HL]);
            context.Emitter.Ld(H, D);
            context.Emitter.Ld(L, E);

            // Find slot in VTable
            context.Emitter.Ld(DE, (byte)((slot * 2) + eeTypeHeader));
            context.Emitter.Add(HL, DE);

            // Get content of slot into HL
            context.Emitter.Ld(E, __[HL]);
            context.Emitter.Inc(HL);
            context.Emitter.Ld(D, __[HL]);
            context.Emitter.Ld(H, D);
            context.Emitter.Ld(L, E);

            // Call (HL)
            context.Emitter.Call("JPHL");
        }
    }
}
