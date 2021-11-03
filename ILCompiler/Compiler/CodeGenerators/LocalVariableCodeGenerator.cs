using ILCompiler.Compiler.EvaluationStack;
using System.Diagnostics;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class LocalVariableCodeGenerator : ICodeGenerator<LocalVariableEntry>
    {
        public void GenerateCode(LocalVariableEntry entry, CodeGeneratorContext context)
        {
            var variable = context.LocalVariableTable[entry.LocalNumber];

            // Loading a local variable/argument
            Debug.Assert(variable.ExactSize % 4 == 0);
            CopyHelper.CopyFromIXToStack(context.Assembler, variable.ExactSize, -variable.StackOffset, restoreIX: true);
        }
    }
}
