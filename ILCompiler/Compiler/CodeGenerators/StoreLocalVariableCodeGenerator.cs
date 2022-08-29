using ILCompiler.Compiler.EvaluationStack;
using System.Diagnostics;
using Z80Assembler;

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

                // pop lsw
                context.Assembler.Pop(R16.HL);

                // pop msw and ignore it as for small data types we
                // truncate the value
                context.Assembler.Pop(R16.DE);

                var ixOffset = -variable.StackOffset;

                // TODO: Deal with offset being outside of +128/-127 range that can be 
                // used with IX indexing address mode

                if (bytesToCopy == 2)
                {
                    context.Assembler.Ld(I16.IX, (short)(ixOffset + 1), R8.H);
                }
                context.Assembler.Ld(I16.IX, (short)(ixOffset + 0), R8.L);
            }
            else
            {
                // Storing a local variable/argument
                Debug.Assert(variable.ExactSize % 2 == 0);
                CopyHelper.CopyFromStackToIX(context.Assembler, variable.ExactSize, -variable.StackOffset, restoreIX: true);
            }
        }
    }
}
