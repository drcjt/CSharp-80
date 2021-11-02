using ILCompiler.Compiler.EvaluationStack;
using System;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    public class UnaryOperatorCodeGenerator
    {
        public static void GenerateCode(UnaryOperator entry, Assembler assembler)
        {
            if (entry.Operation == Operation.Neg)
            {
                assembler.Call("i_neg");
            }
            else
            {
                throw new NotImplementedException($"Unary operator {entry.Operation} not implemented");
            }
        }
    }
}
