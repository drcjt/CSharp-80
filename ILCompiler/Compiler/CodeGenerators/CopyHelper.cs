using System.Diagnostics;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class CopyHelper
    {
        public static void CopyFromStackToIX(Assembler assembler, int size, int ixOffset = 0, bool restoreIX = false)
        {
            int changeToIX = 0;

            var totalBytesToCopy = size;
            int originalIxOffset = ixOffset;
            do
            {
                var bytesToCopy = totalBytesToCopy > 2 ? 2 : totalBytesToCopy;

                // offset has to be -128 to + 127
                while (ixOffset + bytesToCopy > 128)
                {
                    // Need to move IX along to keep stackOffset within -128 to +127 range
                    assembler.Ld(R16.DE, 127);
                    assembler.Add(I16.IX, R16.DE);
                    changeToIX += 127;

                    ixOffset -= 127;
                    size -= 127;
                }
                while (ixOffset < -128)
                {
                    assembler.Ld(R16.DE, -128);
                    assembler.Add(I16.IX, R16.DE);
                    changeToIX -= 128;

                    ixOffset += 128;
                    size += 128;
                }

                switch (bytesToCopy)
                {
                    case 1:
                        assembler.Pop(R16.HL);
                        assembler.Ld(I16.IX, (short)(ixOffset + 0), R8.L);
                        break;
                    case 2:
                    case 4:
                        assembler.Pop(R16.HL);
                        assembler.Ld(I16.IX, (short)(ixOffset + 1), R8.H);
                        assembler.Ld(I16.IX, (short)(ixOffset + 0), R8.L);
                        break;
                }

                ixOffset += 2;
                totalBytesToCopy -= 2;
            } while (ixOffset < size + originalIxOffset);

            if (changeToIX != 0 && restoreIX)
            {
                assembler.Ld(R16.DE, (short)(-changeToIX));
                assembler.Add(I16.IX, R16.DE);
            }
        }

        public static void CopyFromStackToHeap(Assembler assembler, int size, int ixOffset = 0, bool restoreIX = false)
        {
            // Currently only support int32 here
            Debug.Assert(size == 4);

            // Reverse endianness, stack is big endian, heap is little endian
            assembler.Pop(R16.HL);
            assembler.Ld(I16.IX, (short)(ixOffset + 3), R8.H);
            assembler.Ld(I16.IX, (short)(ixOffset + 2), R8.L);

            assembler.Pop(R16.HL);
            assembler.Ld(I16.IX, (short)(ixOffset + 1), R8.H);
            assembler.Ld(I16.IX, (short)(ixOffset + 0), R8.L);
        }

        public static void CopyFromHeapToStack(Assembler assembler, int size, int ixOffset = 0, bool restoreIX = false)
        {
            // Currently only support int32 here
            Debug.Assert(size == 4);

            assembler.Ld(R8.H, I16.IX, (short)(ixOffset + 1));
            assembler.Ld(R8.L, I16.IX, (short)(ixOffset + 0));
            assembler.Push(R16.HL);

            assembler.Ld(R8.H, I16.IX, (short)(ixOffset + 3));
            assembler.Ld(R8.L, I16.IX, (short)(ixOffset + 2));
            assembler.Push(R16.HL);
        }

        public static void CopyFromIXToStack(Assembler assembler, int size, int ixOffset = 0, bool restoreIX = false)
        {
            int changeToIX = 0;

            int originalIxOffset = ixOffset;
            ixOffset += size - 2;
            do
            {
                var bytesToCopy = size > 2 ? 2 : size;
                size -= 2;

                if (ixOffset + bytesToCopy < -127)
                {
                    var delta = ixOffset + 1;
                    assembler.Ld(R16.DE, (short)delta);
                    assembler.Add(I16.IX, R16.DE);
                    changeToIX += delta;

                    ixOffset -= delta;
                    originalIxOffset -= delta;
                }

                switch (bytesToCopy)
                {
                    case 1:
                        assembler.Ld(R8.H, 0);
                        assembler.Ld(R8.L, I16.IX, (short)(ixOffset + 1));
                        assembler.Push(R16.HL);
                        break;

                    case 2:
                    case 4:
                        assembler.Ld(R8.H, I16.IX, (short)(ixOffset + 1));
                        assembler.Ld(R8.L, I16.IX, (short)(ixOffset + 0));
                        assembler.Push(R16.HL);
                        break;
                }

                ixOffset -= 2;
            } while (ixOffset >= originalIxOffset);

            if (changeToIX != 0 && restoreIX)
            {
                assembler.Ld(R16.DE, (short)(-changeToIX));
                assembler.Add(I16.IX, R16.DE);
            }
        }
    }
}
