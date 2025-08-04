using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class UnaryOperatorCodeGenerator : ICodeGenerator<UnaryOperator>
    {
        private static readonly Dictionary<Tuple<Operation, VarType>, string> _operationToInstructionMap = new()
        {
            { Tuple.Create(Operation.Neg, VarType.Ptr), "i_neg16" },
            { Tuple.Create(Operation.Neg, VarType.Int), "i_neg" },
            { Tuple.Create(Operation.Not, VarType.Ptr), "i_not16" },
            { Tuple.Create(Operation.Not, VarType.Int), "i_not" }
        };

        public void GenerateCode(UnaryOperator entry, CodeGeneratorContext context)
        {
            if (_operationToInstructionMap.TryGetValue(Tuple.Create(entry.Operation, entry.Type), out string? instruction))
            {
                context.InstructionsBuilder.Call(instruction);
                return;
            }

            throw new NotImplementedException($"Unary operator {entry.Operation} not implemented");
        }
    }
}
