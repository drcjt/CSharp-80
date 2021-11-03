using ILCompiler.Compiler.EvaluationStack;
using System;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class UnaryOperatorCodeGenerator : ICodeGenerator<UnaryOperator>
    {
        public void GenerateCode(UnaryOperator entry, CodeGeneratorContext context)
        {
            if (entry.Operation == Operation.Neg)
            {
                context.Assembler.Call("i_neg");
            }
            else
            {
                throw new NotImplementedException($"Unary operator {entry.Operation} not implemented");
            }
        }
    }
}
