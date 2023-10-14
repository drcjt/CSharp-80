using ILCompiler.Compiler.EvaluationStack;
using System.Diagnostics;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class StoreLocalVariableCodeGenerator : ICodeGenerator<StoreLocalVariableEntry>
    {
        public void GenerateCode(StoreLocalVariableEntry entry, CodeGeneratorContext context)
        {
            var variable = context.LocalVariableTable[entry.LocalNumber];

            if (variable.Type.IsSmall())
            {
                // Copy from stack to IX truncating to required size
                var bytesToCopy = variable.Type.IsByte() ? 1 : 2;

                CopyHelper.CopyStackToSmall(context.InstructionsBuilder, bytesToCopy, -variable.StackOffset);
            }
            else
            {
                // Storing a local variable/argument
                Debug.Assert(variable.ExactSize % 2 == 0);
                CopyHelper.CopyFromStackToIX(context.InstructionsBuilder, variable.ExactSize, -variable.StackOffset, restoreIX: true);
            }
        }
    }
}
