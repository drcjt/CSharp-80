using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class StoreLocalVariableCodeGenerator : ICodeGenerator<StoreLocalVariableEntry>
    {
        public void GenerateCode(StoreLocalVariableEntry entry, CodeGeneratorContext context)
        {
            var variable = context.LocalVariableTable[entry.LocalNumber];

            if (entry.Op1.Contained)
            {
                GenerateStoreConstantToLocal(context, variable, entry.Op1);
            }
            else if (variable.Type.IsSmall())
            {
                // Copy from stack to IX truncating to required size
                var bytesToCopy = variable.Type.IsByte() ? 1 : 2;

                CopyHelper.CopyStackToSmall(context.InstructionsBuilder, bytesToCopy, -variable.StackOffset);
            }
            else
            {
                // Storing a local variable/argument
                CopyHelper.CopyFromStackToIX(context.InstructionsBuilder, variable.ExactSize, -variable.StackOffset, restoreIX: true);
            }
        }

        /// <summary>
        /// Generate code to store a constant, taking into account the size of the local variable being stored to, and also
        /// dealing with the offset of the local variable lying beyond the immediately addressable range via IX
        /// 
        /// </summary>
        /// <param name="context">used for emitting instructions</param>
        /// <param name="variable">local variable to store to</param>
        /// <param name="constant">constant, can be Int32Constant or NativeIntConstant</param>
        private static void GenerateStoreConstantToLocal(CodeGeneratorContext context, LocalVariableDescriptor variable, StackEntry constant)
        {
            byte[] intBytes = BitConverter.GetBytes(constant.GetIntConstant());

            var ixOffset = -variable.StackOffset;
            int changeToIX = 0;

            // offset has to be -128 to + 127
            if (ixOffset < -128)
            {
                // Move IX so new offset will be 0
                var delta = -ixOffset;
                context.InstructionsBuilder.Ld(DE, (short)-delta);
                context.InstructionsBuilder.Add(IX, DE);
                changeToIX -= delta;

                ixOffset += delta;
            }

            int bytesToStore = variable.Type.IsByte() ? 1 : variable.ExactSize;
            for (int i = 0; i < bytesToStore; i++) 
            {
                context.InstructionsBuilder.Ld(__[IX + (short)(ixOffset + i)], intBytes[i]);
            }

            if (changeToIX != 0)
            {
                context.InstructionsBuilder.Ld(DE, (short)(-changeToIX));
                context.InstructionsBuilder.Add(IX, DE);
            }
        }
    }
}
