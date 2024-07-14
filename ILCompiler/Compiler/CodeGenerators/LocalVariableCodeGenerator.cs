using ILCompiler.Compiler.EvaluationStack;

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
                var signExtend = !variable.Type.IsUnsigned();
                var bytesToCopy = variable.Type.IsByte() ? 1 : 2;

                CopyHelper.CopySmallFromIXToStack(context.InstructionsBuilder, bytesToCopy, -variable.StackOffset, signExtend);

            }
            else
            {
                // Loading a local variable/argument
                CopyHelper.CopyFromIXToStack(context.InstructionsBuilder, size, -variable.StackOffset, restoreIX: true);
            }
        }
    }
}
