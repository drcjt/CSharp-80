using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using System;
using System.Collections.Generic;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    public class ComparisonCodeGenerator
    {
        private static readonly Dictionary<Tuple<Operation, StackValueKind>, string> ComparisonOperatorMappings = new()
        {
            { Tuple.Create(Operation.Eq, StackValueKind.Int32), "i_eq" },
            { Tuple.Create(Operation.Ge, StackValueKind.Int32), "i_ge" },
            { Tuple.Create(Operation.Gt, StackValueKind.Int32), "i_gt" },
            { Tuple.Create(Operation.Le, StackValueKind.Int32), "i_le" },
            { Tuple.Create(Operation.Lt, StackValueKind.Int32), "i_lt" },
            { Tuple.Create(Operation.Ne, StackValueKind.Int32), "i_neq" },
        };

        public static void GenerateCode(BinaryOperator entry, Assembler assembler)
        {
            if (ComparisonOperatorMappings.TryGetValue(Tuple.Create(entry.Operation, entry.Kind), out string? routine))
            {
                assembler.Call(routine);
                // If carry set then push i4 1 else push i4 0
                assembler.Ld(R16.HL, 0);
                assembler.Adc(R16.HL, R16.HL);
                assembler.Push(R16.HL);
                assembler.Ld(R16.HL, 0);
                assembler.Push(R16.HL);
            }
        }
    }
}
