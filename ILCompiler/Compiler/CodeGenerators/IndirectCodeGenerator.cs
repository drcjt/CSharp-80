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
                int offset = (int)entry.Offset;
                if (entry.Op1.Contained && entry.Op1 is LocalVariableAddressEntry lvaAddress)
                {
                    var localVariable = context.LocalVariableTable[lvaAddress.LocalNumber];
                    offset = -localVariable.StackOffset + offset;

                    context.InstructionsBuilder.Push(IX);
                }
                context.InstructionsBuilder.Pop(HL);

                var size = entry.ExactSize ?? 4; // TODO: is 4 the right default size?
                if (entry.Type.IsSmall())
                {
                    CopyHelper.CopySmallFromHLToStack(context.InstructionsBuilder, size, offset, !entry.Type.IsUnsigned());
                }
                else
                {
                    CopyHelper.CopyFromHLToStack(context.InstructionsBuilder, size, offset);
                }
            }
            else
            {
                throw new NotImplementedException($"Indirect of type {entry.Type} not supported");
            }
        }
    }
}
