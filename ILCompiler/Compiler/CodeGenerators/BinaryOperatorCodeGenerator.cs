using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using System;
using System.Collections.Generic;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    public class BinaryOperatorCodeGenerator
    {
        private static readonly Dictionary<Tuple<Operation, StackValueKind>, string> BinaryOperatorMappings = new()
        {
            { Tuple.Create(Operation.Add, StackValueKind.Int32), "i_add" },
            { Tuple.Create(Operation.Add, StackValueKind.NativeInt), "i_add" },
            { Tuple.Create(Operation.Sub, StackValueKind.Int32), "i_sub" },
            { Tuple.Create(Operation.Mul, StackValueKind.Int32), "i_mul" },
            { Tuple.Create(Operation.Div, StackValueKind.Int32), "i_div" },
            { Tuple.Create(Operation.Rem, StackValueKind.Int32), "i_rem" },
            { Tuple.Create(Operation.Div_Un, StackValueKind.Int32), "i_div_un" },
            { Tuple.Create(Operation.Rem_Un, StackValueKind.Int32), "i_rem_un" },
        };

        public static void GenerateCode(BinaryOperator entry, Assembler assembler)
        {
            if (BinaryOperatorMappings.TryGetValue(Tuple.Create(entry.Operation, entry.Kind), out string? routine))
            {
                assembler.Call(routine);
            }
        }
    }
}
