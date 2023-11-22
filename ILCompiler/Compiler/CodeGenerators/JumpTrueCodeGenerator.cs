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
            var condition = binOp.Operation == Operation.Eq ? Condition.Z : Condition.NZ;
            if (binOp.Op1.IsIntegralConstant(0))
            {
                GenerateJumpCompare(entry, context, condition);
            }
            else
            {
                GenerateJumpCompareZero(entry, context, binOp, condition);
            }
        }

        private static void GenerateJumpCompareZero(JumpTrueEntry entry, CodeGeneratorContext context, BinaryOperator binOp, Condition condition)
        {
            if (entry.Condition.Type.GenActualTypeIsI())
            {
                var comparand = binOp.Op1.As<NativeIntConstantEntry>().Value;
                context.InstructionsBuilder.Pop(HL);
                context.InstructionsBuilder.Ld(BC, comparand);
                context.InstructionsBuilder.And(A);
                context.InstructionsBuilder.Sbc(HL, BC);
                context.InstructionsBuilder.Jp(condition, entry.TargetLabel);
            }
            else
            {
                var comparand = binOp.Op1.As<Int32ConstantEntry>().Value;
                var low = BitConverter.ToInt16(BitConverter.GetBytes(comparand), 0);
                var high = BitConverter.ToInt16(BitConverter.GetBytes(comparand), 2);

                if (condition == Condition.NZ)
                {
                    context.InstructionsBuilder.Pop(HL);
                    context.InstructionsBuilder.Ld(BC, low);
                    context.InstructionsBuilder.And(A);
                    context.InstructionsBuilder.Sbc(HL, BC);
                    context.InstructionsBuilder.Pop(HL);
                    context.InstructionsBuilder.Jp(Condition.NZ, entry.TargetLabel);
                    context.InstructionsBuilder.Ld(BC, high);
                    context.InstructionsBuilder.Sbc(HL, BC);
                    context.InstructionsBuilder.Jp(Condition.NZ, entry.TargetLabel);
                }
                else
                {
                    var endLabel = context.NameMangler.GetUniqueName();
                    context.InstructionsBuilder.Pop(HL);
                    context.InstructionsBuilder.Ld(BC, low);
                    context.InstructionsBuilder.And(A);
                    context.InstructionsBuilder.Sbc(HL, BC);
                    context.InstructionsBuilder.Pop(HL);
                    context.InstructionsBuilder.Jr(Condition.NZ, endLabel);
                    context.InstructionsBuilder.Ld(BC, high);
                    context.InstructionsBuilder.Sbc(HL, BC);
                    context.InstructionsBuilder.Jr(Condition.NZ, endLabel);
                    context.InstructionsBuilder.Jp(entry.TargetLabel);
                    context.InstructionsBuilder.Label(endLabel);
                }
            }
        }

        private static void GenerateJumpCompare(JumpTrueEntry entry, CodeGeneratorContext context, Condition condition)
        {
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
            context.InstructionsBuilder.Jp(condition, entry.TargetLabel);
        }
    }
}
