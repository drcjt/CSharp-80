using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;
using System.Diagnostics;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal static class BinaryOperatorCodeGenerator
    {
        private static readonly Dictionary<Tuple<Operation, VarType>, string> BinaryOperatorMappings = new()
        {
            { Tuple.Create(Operation.Add, VarType.Int), "i_add" },
            { Tuple.Create(Operation.Or, VarType.Int), "i_or" },
            { Tuple.Create(Operation.Xor, VarType.Int), "i_xor" },
            { Tuple.Create(Operation.Sub, VarType.Int), "i_sub" },
            { Tuple.Create(Operation.Mul, VarType.Int), "i_mul" },
            { Tuple.Create(Operation.Div, VarType.Int), "i_div" },
            { Tuple.Create(Operation.Rem, VarType.Int), "i_rem" },
            { Tuple.Create(Operation.Div_Un, VarType.Int), "i_div_un" },
            { Tuple.Create(Operation.Rem_Un, VarType.Int), "i_rem_un" },
            { Tuple.Create(Operation.And, VarType.Int), "i_and" },
            { Tuple.Create(Operation.Lsh, VarType.Int), "i_lsh" },
            { Tuple.Create(Operation.Rsh, VarType.Int), "i_rsh" },

            { Tuple.Create(Operation.Add, VarType.Ptr), "i_add16" },
            { Tuple.Create(Operation.Sub, VarType.Ptr), "i_sub16" },
            { Tuple.Create(Operation.Mul, VarType.Ptr), "i_mul16" },
            { Tuple.Create(Operation.Lsh, VarType.Ptr), "i_lsh16" },
            { Tuple.Create(Operation.Rsh, VarType.Ptr), "i_rsh16" },
            { Tuple.Create(Operation.And, VarType.Ptr), "i_and16" },
            { Tuple.Create(Operation.Or, VarType.Ptr), "i_or16" },

            { Tuple.Create(Operation.Sub, VarType.ByRef), "i_sub16" },
        };

        private static bool IsAddOrSub(BinaryOperator op) => op.Operation == Operation.Add || op.Operation == Operation.Sub;
        private static bool IsShift(BinaryOperator op) => op.Operation == Operation.Lsh || op.Operation == Operation.Rsh;
        public static void GenerateCode(BinaryOperator entry, CodeGeneratorContext context)
        {
            Debug.Assert(!entry.IsComparison);

            // Treat all int types as Int
            var operatorType = entry.Type.IsInt() ? VarType.Int : entry.Type;

            if (IsShift(entry))
            {
                if (entry.Op2.IsContainedIntOrI())
                {
                    GenerateContainedIntShift(entry, context);
                    return;
                }
            }

            if (IsAddOrSub(entry))
            {
                if (operatorType == VarType.Ptr || operatorType == VarType.Ref)
                {
                    context.InstructionsBuilder.Pop(HL);

                    if (entry.Op2.IsContainedIntOrI())
                    {
                        int value = entry.Op2.As<NativeIntConstantEntry>().Value;
                        value = entry.Operation == Operation.Sub ? -value : value;

                        if (value == 1)
                        {
                            context.InstructionsBuilder.Inc(HL);
                        }
                        else if (value == -1)
                        {
                            context.InstructionsBuilder.Dec(HL);
                        }
                        else
                        {
                            context.InstructionsBuilder.Ld(BC, (short)entry.Op2.GetIntConstant());
                            GenerateAddOrSub(context, entry.Operation);
                        }
                    }
                    else
                    {
                        context.InstructionsBuilder.Pop(BC);
                        GenerateAddOrSub(context, entry.Operation);
                    }
                    context.InstructionsBuilder.Push(HL);
                    return;
                }

                if (entry.Op2.IsContainedIntOrI())
                {
                    GenerateContainedIntAddOrSub(entry, context);
                    return;
                }
            }

            if (BinaryOperatorMappings.TryGetValue(Tuple.Create(entry.Operation, operatorType), out string? routine))
            {
                context.InstructionsBuilder.Call(routine);
                return;
            }

            throw new NotImplementedException($"Binary operator {entry.Operation} for type {operatorType} not yet implemented");
        }

        private static void GenerateContainedIntShift(BinaryOperator entry, CodeGeneratorContext context)
        {
            switch (entry.Operation)
            {
                case Operation.Lsh: GenerateContainedIntLeftShift(entry, context); break;
                case Operation.Rsh: GenerateContainedIntRightShift(entry, context); break;
                default: throw new InvalidOperationException();
            };
        }

        private static void GenerateContainedIntLeftShift(BinaryOperator entry, CodeGeneratorContext context)
        {
            int value = entry.Op2.As<Int32ConstantEntry>().Value;

            context.InstructionsBuilder.Pop(HL);
            context.InstructionsBuilder.Pop(DE);

            if (value == 1)
            {

                context.InstructionsBuilder.Ld(A, E);
                context.InstructionsBuilder.Add(HL, HL);
                context.InstructionsBuilder.Rla();
                context.InstructionsBuilder.Rl(D);
                context.InstructionsBuilder.Ld(E, A);
            }
            else if (value == 8)
            {
                context.InstructionsBuilder.Ld(D, E);
                context.InstructionsBuilder.Ld(E, H);
                context.InstructionsBuilder.Ld(H, L);
                context.InstructionsBuilder.Ld(L, 0);
            }
            else if (value == 16)
            {
                context.InstructionsBuilder.Ld(D, H);
                context.InstructionsBuilder.Ld(E, L);
                context.InstructionsBuilder.Ld(HL, 0);
            }
            else if (value > 31)
            {
                context.InstructionsBuilder.Ld(DE, 0);
                context.InstructionsBuilder.Ld(HL, 0);
            }
            else
            {
                throw new InvalidOperationException();
            }

            context.InstructionsBuilder.Push(DE);
            context.InstructionsBuilder.Push(HL);
        }

        private static void GenerateContainedIntRightShift(BinaryOperator entry, CodeGeneratorContext context)
        {
            int value = entry.Op2.As<Int32ConstantEntry>().Value;

            context.InstructionsBuilder.Pop(HL);
            context.InstructionsBuilder.Pop(DE);

            if (value == 1)
            {
                context.InstructionsBuilder.Ld(A, E);
                context.InstructionsBuilder.Sra(D);
                context.InstructionsBuilder.Rra();
                context.InstructionsBuilder.Rr(H);
                context.InstructionsBuilder.Rr(L);
                context.InstructionsBuilder.Ld(E, A);
            }
            else if (value == 8)
            {
                context.InstructionsBuilder.Ld(L, H);
                context.InstructionsBuilder.Ld(H, E);
                context.InstructionsBuilder.Ld(E, D);
                context.InstructionsBuilder.Ld(D, 0);
            }
            else if (value == 16)
            {
                context.InstructionsBuilder.Ld(H, D);
                context.InstructionsBuilder.Ld(L, E);
                context.InstructionsBuilder.Ld(DE, 0);
            }
            else if (value > 31)
            {
                context.InstructionsBuilder.Ld(HL, 0);
                context.InstructionsBuilder.Ld(DE, 0);
            }
            else
            {
                throw new InvalidOperationException();
            }

            context.InstructionsBuilder.Push(DE);
            context.InstructionsBuilder.Push(HL);
        }

        private static void GenerateContainedIntAddOrSub(BinaryOperator entry, CodeGeneratorContext context)
        {
            int value = entry.Op2.GetIntConstant();

            if (entry.Operation == Operation.Sub)
            {
                value = -value;
            }

            var low = BitConverter.ToInt16(BitConverter.GetBytes(value), 0);
            var high = BitConverter.ToInt16(BitConverter.GetBytes(value), 2);

            // Try to use an increment or decrement
            if (value == 1)
            {
                GenerateInc(context);
                return;
            }
            if (value == -1)
            {
                GenerateDec(context);
                return;
            }

            // Adding constant so can inline this
            context.InstructionsBuilder.Pop(HL);
            context.InstructionsBuilder.Pop(DE);
            context.InstructionsBuilder.Ld(BC, low);
            context.InstructionsBuilder.Add(HL, BC);
            context.InstructionsBuilder.Ex(DE, HL);
            context.InstructionsBuilder.Ld(BC, high);
            context.InstructionsBuilder.Adc(HL, BC);
            context.InstructionsBuilder.Push(HL);
            context.InstructionsBuilder.Push(DE);
        }

        private static void GenerateInc(CodeGeneratorContext context)
        {
            var endLabel = context.NameMangler.GetUniqueName();
            context.InstructionsBuilder.Pop(HL);
            context.InstructionsBuilder.Pop(DE);
            context.InstructionsBuilder.Inc(L);
            context.InstructionsBuilder.Jr(Condition.NZ, endLabel);
            context.InstructionsBuilder.Inc(H);
            context.InstructionsBuilder.Jr(Condition.NZ, endLabel);
            context.InstructionsBuilder.Inc(DE);
            context.InstructionsBuilder.Label(endLabel);
            context.InstructionsBuilder.Push(DE);
            context.InstructionsBuilder.Push(HL);
        }

        private static void GenerateDec(CodeGeneratorContext context)
        {
            var endLabel = context.NameMangler.GetUniqueName();
            context.InstructionsBuilder.Pop(HL);
            context.InstructionsBuilder.Pop(DE);
            context.InstructionsBuilder.Ld(A, H);
            context.InstructionsBuilder.Or(L);
            context.InstructionsBuilder.Dec(HL);
            context.InstructionsBuilder.Jr(Condition.NZ, endLabel);
            context.InstructionsBuilder.Dec(DE);
            context.InstructionsBuilder.Label(endLabel);
            context.InstructionsBuilder.Push(DE);
            context.InstructionsBuilder.Push(HL);
        }

        private static void GenerateAddOrSub(CodeGeneratorContext context, Operation op)
        {
            if (op == Operation.Add)
            {
                context.InstructionsBuilder.Add(HL, BC);
            }
            else
            {
                context.InstructionsBuilder.Or(A);
                context.InstructionsBuilder.Sbc(HL, BC);
            }
        }
    }
}
