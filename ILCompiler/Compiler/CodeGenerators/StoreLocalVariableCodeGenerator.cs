using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Common.TypeSystem.IL;
using System.Diagnostics;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class StoreLocalVariableCodeGenerator : ICodeGenerator<StoreLocalVariableEntry>
    {
        public void GenerateCode(StoreLocalVariableEntry entry, CodeGeneratorContext context)
        {
            var variable = context.LocalVariableTable[entry.LocalNumber];

            // Storing a local variable/argument
            Debug.Assert(variable.ExactSize % 2 == 0);
            CopyHelper.CopyFromStackToIX(context.Assembler, variable.ExactSize, -variable.StackOffset, restoreIX: true);
        }
    }
}
