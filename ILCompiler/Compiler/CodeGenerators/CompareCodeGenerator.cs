using System.Diagnostics;
using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal static class CompareCodeGenerator
    {
        private static readonly Dictionary<Tuple<Operation, VarType>, string> ComparisonOperatorMappings = new()
        {
            { Tuple.Create(Operation.Eq, VarType.Int), "i_eq" },
            { Tuple.Create(Operation.Ge, VarType.Int), "i_ge" },
            { Tuple.Create(Operation.Ge_Un, VarType.Int), "i_ge_un" },
            { Tuple.Create(Operation.Gt, VarType.Int), "i_gt" },
            { Tuple.Create(Operation.Gt_Un, VarType.Int), "i_gt_un" },
            { Tuple.Create(Operation.Ne_Un, VarType.Int), "i_neq" },

            { Tuple.Create(Operation.Ne_Un, VarType.Ptr), "i_neq16" },
            { Tuple.Create(Operation.Eq, VarType.Ptr), "i_eq16" },
            { Tuple.Create(Operation.Ge_Un, VarType.Ptr), "i_ge16_un" },
            { Tuple.Create(Operation.Gt_Un, VarType.Ptr), "i_gt16_un" },
            { Tuple.Create(Operation.Gt, VarType.Ptr), "i_gt16" },
            { Tuple.Create(Operation.Ge, VarType.Ptr), "i_ge16" },

            { Tuple.Create(Operation.Ne_Un, VarType.Ref), "i_neq16" },
            { Tuple.Create(Operation.Eq, VarType.Ref), "i_eq16" },
            { Tuple.Create(Operation.Gt_Un, VarType.Ref), "i_gt16_un" },

            { Tuple.Create(Operation.Ne_Un, VarType.ByRef), "i_neq16" },
            { Tuple.Create(Operation.Eq, VarType.ByRef), "i_eq16" },
        };

        public static void GenerateCode(BinaryOperator entry, CodeGeneratorContext context)
        {
            Debug.Assert(entry.IsComparison);

            Debug.Assert(entry.Operation != Operation.Lt);
            Debug.Assert(entry.Operation != Operation.Le);
            Debug.Assert(entry.Operation != Operation.Lt_Un);
            Debug.Assert(entry.Operation != Operation.Le_Un);

            var op1Type = entry.Op1.Type.IsInt() ? VarType.Int : entry.Op1.Type;

            if (ComparisonOperatorMappings.TryGetValue(Tuple.Create(entry.Operation, op1Type), out string? routine))
            {
                if (entry.Operation == Operation.Eq && entry.Op2.IsContainedIntOrI() && entry.Op2.IsIntegralConstant(0))
                {
                    GenerateEqualsZero(context, op1Type);
                }
                else
                {
                    context.InstructionsBuilder.Call(routine);
                }

                if (!entry.ResultUsedInJump)
                {
                    // If carry set then push i4 1 else push i4 0
                    context.InstructionsBuilder.Ld(HL, 0);
                    context.InstructionsBuilder.Push(HL);     // MSW

                    context.InstructionsBuilder.Ld(HL, 0);
                    context.InstructionsBuilder.Adc(HL, HL);
                    context.InstructionsBuilder.Push(HL);     // LSW
                }
            }
            else
            {
                throw new NotImplementedException($"Binary operator {entry.Operation} for type {op1Type} not yet implemented");
            }
        }

        private static void GenerateEqualsZero(CodeGeneratorContext context, VarType op1Type)
        {
            if (op1Type == VarType.Ptr || op1Type == VarType.Ref)
            {
                context.InstructionsBuilder.Pop(HL);
                context.InstructionsBuilder.Ld(A, H);
                context.InstructionsBuilder.Or(L);

                SetCarryFlagBasedOnZeroFlag(context);
            }
            else
            {
                context.InstructionsBuilder.Pop(HL);
                context.InstructionsBuilder.Pop(DE);
                context.InstructionsBuilder.Ld(A, D);
                context.InstructionsBuilder.Or(E);
                context.InstructionsBuilder.Or(H);
                context.InstructionsBuilder.Or(L);

                SetCarryFlagBasedOnZeroFlag(context);
            }
        }

        private static void SetCarryFlagBasedOnZeroFlag(CodeGeneratorContext context)
        {
            var notZeroLabel = context.NameMangler.GetUniqueName();
            var endLabel = context.NameMangler.GetUniqueName();

            context.InstructionsBuilder.Jr(Emit.Condition.NZ, notZeroLabel);
            context.InstructionsBuilder.Scf();
            context.InstructionsBuilder.Jr(endLabel);
            context.InstructionsBuilder.Label(notZeroLabel);
            context.InstructionsBuilder.Xor(A); // Clear carry flag
            context.InstructionsBuilder.Label(endLabel);
        }
    }
}
