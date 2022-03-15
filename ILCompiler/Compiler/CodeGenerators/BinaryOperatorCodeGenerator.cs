using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class BinaryOperatorCodeGenerator : ICodeGenerator<BinaryOperator>
    {
        private static readonly Dictionary<Tuple<Operation, StackValueKind>, string> BinaryOperatorMappings = new()
        {
            { Tuple.Create(Operation.Add, StackValueKind.Int32), "i_add" },
            { Tuple.Create(Operation.Sub, StackValueKind.Int32), "i_sub" },
            { Tuple.Create(Operation.Mul, StackValueKind.Int32), "i_mul" },
            { Tuple.Create(Operation.Div, StackValueKind.Int32), "i_div" },
            { Tuple.Create(Operation.Rem, StackValueKind.Int32), "i_rem" },
            { Tuple.Create(Operation.Div_Un, StackValueKind.Int32), "i_div_un" },
            { Tuple.Create(Operation.Rem_Un, StackValueKind.Int32), "i_rem_un" },
            { Tuple.Create(Operation.Lsh, StackValueKind.Int32), "i_lsh" },
            { Tuple.Create(Operation.Rsh, StackValueKind.Int32), "i_rsh" },

            { Tuple.Create(Operation.Add, StackValueKind.NativeInt), "i_add16" },
            { Tuple.Create(Operation.Add, StackValueKind.NativeInt), "i_mul16" },
            { Tuple.Create(Operation.Lsh, StackValueKind.NativeInt), "i_lsh16" },
            { Tuple.Create(Operation.Rsh, StackValueKind.NativeInt), "i_rsh16" },
        };

        private static readonly Dictionary<Tuple<Operation, StackValueKind>, string> ComparisonOperatorMappings = new()
        {
            { Tuple.Create(Operation.Eq, StackValueKind.Int32), "i_eq" },
            { Tuple.Create(Operation.Ge, StackValueKind.Int32), "i_ge" },
            { Tuple.Create(Operation.Gt, StackValueKind.Int32), "i_gt" },
            { Tuple.Create(Operation.Le, StackValueKind.Int32), "i_le" },
            { Tuple.Create(Operation.Lt, StackValueKind.Int32), "i_lt" },
            { Tuple.Create(Operation.Ne, StackValueKind.Int32), "i_neq" },
        };

        public void GenerateCode(BinaryOperator entry, CodeGeneratorContext context)
        {
            if (entry.IsComparison)
            {
                if (ComparisonOperatorMappings.TryGetValue(Tuple.Create(entry.Operation, entry.Kind), out string? routine))
                {
                    context.Assembler.Call(routine);
                    // If carry set then push i4 1 else push i4 0
                    context.Assembler.Ld(R16.HL, 0);
                    context.Assembler.Push(R16.HL);     // MSW

                    context.Assembler.Ld(R16.HL, 0);
                    context.Assembler.Adc(R16.HL, R16.HL);
                    context.Assembler.Push(R16.HL);     // LSW
                }
            }
            else
            {
                if (BinaryOperatorMappings.TryGetValue(Tuple.Create(entry.Operation, entry.Kind), out string? routine))
                {
                    context.Assembler.Call(routine);
                }
                else
                {
                    throw new NotImplementedException("Binary operator not yet implemented");
                }
            }
        }
    }
}
