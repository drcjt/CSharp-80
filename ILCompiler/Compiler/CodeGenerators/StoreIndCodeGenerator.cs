using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class StoreIndCodeGenerator : ICodeGenerator<StoreIndEntry>
    {
        public void GenerateCode(StoreIndEntry entry, CodeGeneratorContext context)
        {
            var exactSize = entry.ExactSize ?? 0;
            if (exactSize > 0)
            {
                context.Emitter.Push(I16.IX);
                context.Emitter.Pop(R16.BC);

                context.Emitter.Pop(I16.IX);  

                short offset = (short)entry.FieldOffset;

                if (entry.Type.IsSmall())
                {
                    CopyHelper.CopyStackToSmall(context.Emitter, exactSize, offset);
                }
                else
                {
                    CopyHelper.CopyFromStackToIX(context.Emitter, exactSize, offset);
                }

                context.Emitter.Push(R16.BC);
                context.Emitter.Pop(I16.IX);
            }
        }
    }
}
