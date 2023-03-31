using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class BinaryOperatorCodeGenerator : ICodeGenerator<BinaryOperator>
    {
        private static readonly Dictionary<Tuple<Operation, VarType>, string> BinaryOperatorMappings = new()
        {
            { Tuple.Create(Operation.Add, VarType.Int), "i_add" },
            { Tuple.Create(Operation.Or, VarType.Int), "i_or" },
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
            { Tuple.Create(Operation.Mul, VarType.Ptr), "i_mul16" },
            { Tuple.Create(Operation.Lsh, VarType.Ptr), "i_lsh16" },
            { Tuple.Create(Operation.Rsh, VarType.Ptr), "i_rsh16" },
            { Tuple.Create(Operation.And, VarType.Ptr), "i_and16" },
            { Tuple.Create(Operation.Or, VarType.Ptr), "i_or16" },
        };

        private static readonly Dictionary<Tuple<Operation, VarType>, string> ComparisonOperatorMappings = new()
        {
            { Tuple.Create(Operation.Eq, VarType.Int), "i_eq" },
            { Tuple.Create(Operation.Ge, VarType.Int), "i_ge" },
            { Tuple.Create(Operation.Gt, VarType.Int), "i_gt" },
            { Tuple.Create(Operation.Gt_Un, VarType.Int), "i_gt_un" },
            { Tuple.Create(Operation.Le, VarType.Int), "i_le" },
            { Tuple.Create(Operation.Lt, VarType.Int), "i_lt" },
            { Tuple.Create(Operation.Lt_Un, VarType.Int), "i_lt_un" },
            { Tuple.Create(Operation.Ne_Un, VarType.Int), "i_neq" },

            { Tuple.Create(Operation.Ne_Un, VarType.Ptr), "i_neq16" },
            { Tuple.Create(Operation.Eq, VarType.Ptr), "i_eq16" },
            { Tuple.Create(Operation.Lt_Un, VarType.Ptr), "i_lt_un16" },

            { Tuple.Create(Operation.Ne_Un, VarType.Ref), "i_neq16" },
            { Tuple.Create(Operation.Eq, VarType.Ref), "i_eq16" },

            { Tuple.Create(Operation.Ne_Un, VarType.ByRef), "i_neq16" },
            { Tuple.Create(Operation.Eq, VarType.ByRef), "i_eq16" },
        };

        public void GenerateCode(BinaryOperator entry, CodeGeneratorContext context)
        {
            // Treat all int types as Int
            var operatorType = entry.Type.IsInt() ? VarType.Int : entry.Type;

            if (entry.IsComparison)
            {
                var op1Type = entry.Op1.Type.IsInt() ? VarType.Int : entry.Op1.Type;

                if (ComparisonOperatorMappings.TryGetValue(Tuple.Create(entry.Operation, op1Type), out string? routine))
                {
                    context.Emitter.Call(routine);
                    // If carry set then push i4 1 else push i4 0
                    context.Emitter.Ld(R16.HL, 0);
                    context.Emitter.Push(R16.HL);     // MSW

                    context.Emitter.Ld(R16.HL, 0);
                    context.Emitter.Adc(R16.HL, R16.HL);
                    context.Emitter.Push(R16.HL);     // LSW
                }
                else
                {
                    throw new NotImplementedException($"Binary operator {entry.Operation} for type {entry.Type} not yet implemented");
                }
            }
            else
            {
                if (BinaryOperatorMappings.TryGetValue(Tuple.Create(entry.Operation, operatorType), out string? routine))
                {
                    context.Emitter.Call(routine);
                }
                else
                {
                    throw new NotImplementedException($"Binary operator {entry.Operation} for type {entry.Type} not yet implemented");
                }
            }
        }
    }
}
