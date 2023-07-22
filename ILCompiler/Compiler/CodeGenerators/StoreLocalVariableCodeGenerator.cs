using ILCompiler.Compiler.EvaluationStack;
using System.Diagnostics;
using static ILCompiler.Compiler.Emit.Registers;

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

                // Address to copy to needs to be in HL
                context.Emitter.Push(IX);
                context.Emitter.Pop(HL);

                CopyHelper.CopyStackToSmall(context.Emitter, bytesToCopy, (short)-variable.StackOffset);
            }
            else
            {
                // Storing a local variable/argument
                Debug.Assert(variable.ExactSize % 2 == 0);

                // Address to copy to needs to be in HL
                context.Emitter.Push(IX);
                context.Emitter.Pop(HL);

                CopyHelper.CopyFromStackToHL(context.Emitter, variable.ExactSize, (short)-variable.StackOffset);
            }
        }
    }
}
