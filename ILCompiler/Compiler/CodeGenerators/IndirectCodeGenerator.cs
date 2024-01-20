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
                var size = entry.ExactSize ?? 4; // TODO: is 4 the right default size?

                context.InstructionsBuilder.Pop(HL);
                if (entry.Type.IsSmall())
                {
                    CopyHelper.CopySmallFromHLToStack(context.InstructionsBuilder, size, (short)entry.Offset, !entry.Type.IsUnsigned());
                }
                else
                {
                    CopyHelper.CopyFromHLToStack(context.InstructionsBuilder, size, (short)entry.Offset);
                }
            }
            else
            {
                throw new NotImplementedException($"Indirect of type {entry.Type} not supported");
            }
        }
    }
}
