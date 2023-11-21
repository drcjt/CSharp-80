using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class JumpTrueCodeGenerator : ICodeGenerator<JumpTrueEntry>
    {
        public void GenerateCode(JumpTrueEntry entry, CodeGeneratorContext context)
        {
            var binOp = (BinaryOperator)entry.Condition;

            if (binOp.Op1.Contained)
            {
                GenerateJumpCompareCode(entry, context, binOp);
                return;
            }


            // Pop i4 from stack and jump if non zero
            context.InstructionsBuilder.Pop(HL);      // LSW
            context.InstructionsBuilder.Ld(A, 0);
            context.InstructionsBuilder.Add(A, L);
            context.InstructionsBuilder.Pop(HL);      // MSW
            context.InstructionsBuilder.Jp(Condition.NZ, entry.TargetLabel);
        }

        private static void GenerateJumpCompareCode(JumpTrueEntry entry, CodeGeneratorContext context, BinaryOperator binOp)
        {
            // Compare to 0 and jump
            if (entry.Condition.Type.GenActualTypeIsI())
            {
                context.InstructionsBuilder.Pop(HL);
                context.InstructionsBuilder.Ld(A, H);
                context.InstructionsBuilder.Or(L);
            }
            else
            {
                context.InstructionsBuilder.Pop(HL);
                context.InstructionsBuilder.Pop(BC);
                context.InstructionsBuilder.And(A);
                context.InstructionsBuilder.Sbc(HL, BC);
            }

            var condition = binOp.Operation == Operation.Eq ? Condition.Z : Condition.NZ;
            context.InstructionsBuilder.Jp(condition, entry.TargetLabel);
        }
    }
}
