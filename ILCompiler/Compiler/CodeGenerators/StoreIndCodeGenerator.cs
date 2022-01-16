using ILCompiler.Compiler.EvaluationStack;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class StoreIndCodeGenerator : ICodeGenerator<StoreIndEntry>
    {
        public void GenerateCode(StoreIndEntry entry, CodeGeneratorContext context)
        {
            var exactSize = entry.ExactSize ?? 0;
            if (exactSize > 0)
            {
                context.Assembler.Pop(R16.HL);  // LSW
                context.Assembler.Pop(R16.BC);  // MSW Address is stored as 32 bits but will only use lsw

                context.Assembler.Push(I16.IX);
                context.Assembler.Pop(R16.BC);

                context.Assembler.Push(R16.HL);
                context.Assembler.Pop(I16.IX);

                short offset = (short)entry.FieldOffset;
                CopyHelper.CopyFromStackToIX(context.Assembler, exactSize, offset);

                context.Assembler.Push(R16.BC);
                context.Assembler.Pop(I16.IX);
            }
        }
    }
}
