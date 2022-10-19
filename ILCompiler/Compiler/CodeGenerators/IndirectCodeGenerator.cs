using ILCompiler.Compiler.EvaluationStack;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class IndirectCodeGenerator : ICodeGenerator<IndirectEntry>
    {
        public void GenerateCode(IndirectEntry entry, CodeGeneratorContext context)
        {
            if (entry.Type.IsInt() || entry.Type == VarType.Struct || entry.Type == VarType.Ptr || entry.Type == VarType.Ref)
            {
                // Save IX to BC
                context.Assembler.Push(I16.IX);
                context.Assembler.Pop(R16.BC);

                // Get indirect address from stack into IX
                context.Assembler.Pop(I16.IX);

                var size = entry.ExactSize ?? 4; // TODO: is 4 the right default size?

                if (entry.Type == VarType.Ptr || entry.Type == VarType.Ref)
                {
                    CopyHelper.CopyFromIXToStack(context.Assembler, size, (short)entry.Offset, false);
                }
                else if (entry.Type.IsSmall())
                {
                    CopyHelper.CopySmallToStack(context.Assembler, size, (short)entry.Offset, !entry.Type.IsUnsigned());
                }
                else
                {
                    CopyHelper.CopyFromIXToStack(context.Assembler, size, (short)entry.Offset, false);
                }

                // Restore IX
                context.Assembler.Push(R16.BC);
                context.Assembler.Pop(I16.IX);
            }
            else
            {
                throw new NotImplementedException($"Indirect of type {entry.Type} not supported");
            }
        }
    }
}
