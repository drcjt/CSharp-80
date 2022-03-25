using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Common.TypeSystem.IL;
using System.Diagnostics;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class LocalVariableCodeGenerator : ICodeGenerator<LocalVariableEntry>
    {
        public void GenerateCode(LocalVariableEntry entry, CodeGeneratorContext context)
        {
            var variable = context.LocalVariableTable[entry.LocalNumber];
            var size = variable.ExactSize;

            // Loading a local variable/argument
            Debug.Assert(size % 2 == 0);
            CopyHelper.CopyFromIXToStack(context.Assembler, size, -variable.StackOffset, restoreIX: true);
        }
    }
}
