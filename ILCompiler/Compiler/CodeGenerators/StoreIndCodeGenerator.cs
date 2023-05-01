using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class StoreIndCodeGenerator : ICodeGenerator<StoreIndEntry>
    {
        public void GenerateCode(StoreIndEntry entry, CodeGeneratorContext context)
        {
            var exactSize = entry.ExactSize ?? 0;
            if (exactSize > 0)
            {
                context.Emitter.Push(IX);
                context.Emitter.Pop(BC);

                context.Emitter.Pop(IX);  

                short offset = (short)entry.FieldOffset;

                if (entry.Type.IsSmall())
                {
                    CopyHelper.CopyStackToSmall(context.Emitter, exactSize, offset);
                }
                else
                {
                    CopyHelper.CopyFromStackToIX(context.Emitter, exactSize, offset);
                }

                context.Emitter.Push(BC);
                context.Emitter.Pop(IX);
            }
        }
    }
}
