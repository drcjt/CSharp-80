using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class IndirectCodeGenerator : ICodeGenerator<IndirectEntry>
    {
        public void GenerateCode(IndirectEntry entry, CodeGeneratorContext context)
        {
            if (entry.Type.IsInt() || entry.Type == VarType.Struct || entry.Type == VarType.Ptr || entry.Type == VarType.Ref)
            {
                // Save IX to BC
                context.InstructionsBuilder.Push(IX);
                context.InstructionsBuilder.Pop(BC);

                // Get indirect address from stack into IX
                context.InstructionsBuilder.Pop(IX);

                var size = entry.ExactSize ?? 4; // TODO: is 4 the right default size?

                if (entry.Type == VarType.Ptr || entry.Type == VarType.Ref)
                {
                    CopyHelper.CopyFromIXToStack(context.InstructionsBuilder, size, (short)entry.Offset, false);
                }
                else if (entry.Type.IsSmall())
                {
                    CopyHelper.CopySmallToStack(context.InstructionsBuilder, size, (short)entry.Offset, !entry.Type.IsUnsigned());
                }
                else
                {
                    CopyHelper.CopyFromIXToStack(context.InstructionsBuilder, size, (short)entry.Offset, false);
                }

                // Restore IX
                context.InstructionsBuilder.Push(BC);
                context.InstructionsBuilder.Pop(IX);
            }
            else
            {
                throw new NotImplementedException($"Indirect of type {entry.Type} not supported");
            }
        }
    }
}
