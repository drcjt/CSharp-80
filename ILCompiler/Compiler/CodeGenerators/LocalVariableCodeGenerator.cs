using ILCompiler.Compiler.EvaluationStack;
using System.Diagnostics;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class LocalVariableCodeGenerator : ICodeGenerator<LocalVariableEntry>
    {
        public void GenerateCode(LocalVariableEntry entry, CodeGeneratorContext context)
        {
            var variable = context.LocalVariableTable[entry.LocalNumber];
            var size = variable.ExactSize;

            if (variable.Type.IsSmall())
            {
                CopyHelper.CopySmallFromIXToStack(context.InstructionsBuilder, variable.Type.IsByte() ? 1 : 2, -variable.StackOffset, !variable.Type.IsUnsigned());
            }
            else
            {
                // Loading a local variable/argument
                Debug.Assert(size % 2 == 0);
                CopyHelper.CopyFromIXToStack(context.InstructionsBuilder, size, -variable.StackOffset, restoreIX: true);
            }
        }
    }
}
