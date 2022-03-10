using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class IndirectCodeGenerator : ICodeGenerator<IndirectEntry>
    {
        public void GenerateCode(IndirectEntry entry, CodeGeneratorContext context)
        {
            if (entry.Kind == StackValueKind.Int32 || entry.Kind == StackValueKind.ValueType || entry.Kind == StackValueKind.NativeInt)
            {
                // Get indirect address from stack into IX, pop lsw then msw
                context.Assembler.Pop(I16.IY);  // LSW
                context.Assembler.Pop(R16.DE);  // Ignore msw of address

                var size = entry.ExactSize ?? 4;
                CopyHelper.CopyFromIYToStack(context.Assembler, size, (short)entry.Offset);

            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
