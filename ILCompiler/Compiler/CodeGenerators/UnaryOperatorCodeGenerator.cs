﻿using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class UnaryOperatorCodeGenerator : ICodeGenerator<UnaryOperator>
    {
        public void GenerateCode(UnaryOperator entry, CodeGeneratorContext context)
        {
            switch (entry.Operation)
            {
                case Operation.Neg:
                    context.InstructionsBuilder.Call("i_neg");
                    break;
                case Operation.Not:
                    context.InstructionsBuilder.Call("i_not");
                    break;
                default:
                    throw new NotImplementedException($"Unary operator {entry.Operation} not implemented");
            }
        }
    }
}
