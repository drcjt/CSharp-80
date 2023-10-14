using ILCompiler.Compiler.Emit;
using System.Diagnostics;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal static class CopyHelper
    {
        public static void CopyStackToSmall(InstructionsBuilder instructionsBuilder, int bytesToCopy, int ixOffset)
        {
            // pop lsw
            instructionsBuilder.Pop(HL);

            // pop msw and ignore it as for small data types we
            // truncate the value
            instructionsBuilder.Pop(DE);

            short changeToIX = 0;
            if (ixOffset + bytesToCopy - 1 > 127)
            {
                // Make IX larger so offset doesn't fall outside of bounds
                changeToIX = 128;
            }
            if (ixOffset < -128)
            {
                changeToIX = -128;
                // Make IX smaller so offset doesn't fall outside of bounds
            }
            if (changeToIX != 0)
            {
                instructionsBuilder.Ld(DE, changeToIX);
                instructionsBuilder.Add(IX, DE);
                ixOffset -= changeToIX;
            }

            if (bytesToCopy == 2)
            {
                instructionsBuilder.Ld(__[IX + (short)(ixOffset + 1)], H);
            }
            instructionsBuilder.Ld(__[IX + (short)(ixOffset + 0)], L);

            if (changeToIX != 0)
            {
                instructionsBuilder.Ld(DE, (short)-changeToIX);
                instructionsBuilder.Add(IX, DE);
            }
        }

        /*
         * Assumes address to copy to is in HL
         */
        public static void CopyStackToHLSmall(InstructionsBuilder instructionsBuilder, int bytesToCopy, int offset)
        {
            if (offset != 0)
            {
                instructionsBuilder.Ld(DE, (short)offset);
                instructionsBuilder.Add(HL, DE);
            }

            instructionsBuilder.Pop(DE);
            instructionsBuilder.Pop(BC);    // Ignore msw

            instructionsBuilder.Ld(__[HL], E);
            instructionsBuilder.Inc(HL);

            if (bytesToCopy == 2)
            {
                instructionsBuilder.Ld(__[HL], D);
            }
        }

        public static void CopySmallToStack(InstructionsBuilder instructionsBuilder, int bytesToCopy, int ixOffset, bool signExtend)
        {
            Debug.Assert(bytesToCopy == 1 || bytesToCopy == 2);
            int changeToIX = 0;

            if (ixOffset + bytesToCopy < -127)
            {
                var delta = ixOffset + 1;
                instructionsBuilder.Ld(DE, (short)delta);
                instructionsBuilder.Add(IX, DE);
                changeToIX += delta;

                ixOffset -= delta;
            }

            if (bytesToCopy == 1)
            {
                if (signExtend)
                {
                    instructionsBuilder.Ld(A, __[IX + (short)(ixOffset)]);
                    instructionsBuilder.Ld(E, A);

                    instructionsBuilder.Add(A, A);
                    instructionsBuilder.Sbc(A, A);
                    instructionsBuilder.Ld(H, A);
                    instructionsBuilder.Ld(L, A);
                    instructionsBuilder.Push(HL);

                    instructionsBuilder.Ld(L, E);
                    instructionsBuilder.Push(HL);
                }
                else
                {
                    instructionsBuilder.Ld(HL, 0);
                    instructionsBuilder.Push(HL);

                    instructionsBuilder.Ld(H, 0);
                    instructionsBuilder.Ld(L, __[IX + (short)(ixOffset)]);
                    instructionsBuilder.Push(HL);
                }
            }
            else
            {
                if (signExtend)
                {
                    instructionsBuilder.Ld(H, __[IX + (short)(ixOffset + 1)]);
                    instructionsBuilder.Ld(L, __[IX + (short)(ixOffset)]);

                    instructionsBuilder.Ld(D, H);
                    instructionsBuilder.Ld(E, L);

                    instructionsBuilder.Add(HL, HL);  // move sign bit into carry flag
                    instructionsBuilder.Sbc(HL, HL);  // hl is now 0000 or FFFF
                    instructionsBuilder.Push(HL);

                    instructionsBuilder.Push(DE);
                }
                else
                {
                    instructionsBuilder.Ld(HL, 0);
                    instructionsBuilder.Push(HL);

                    instructionsBuilder.Ld(H, __[IX + (short)(ixOffset + 1)]);
                    instructionsBuilder.Ld(L, __[IX + (short)(ixOffset)]);
                    instructionsBuilder.Push(HL);
                }
            }

            if (changeToIX != 0)
            {
                instructionsBuilder.Ld(DE, (short)(-changeToIX));
                instructionsBuilder.Add(IX, DE);
            }
        }

        /*
         *  Assumes address to copy to is in HL
         */
        public static void CopyFromStackToHL(InstructionsBuilder instructionsBuilder, int size, int offset = 0)
        {
            if (offset != 0)
            {
                instructionsBuilder.Ld(DE, (short)offset);
                instructionsBuilder.Add(HL, DE);
            }

            var totalBytesToCopy = size;
            do
            {
                var bytesToCopy = totalBytesToCopy > 2 ? 2 : totalBytesToCopy;
                totalBytesToCopy -= 2;

                instructionsBuilder.Pop(DE);

                instructionsBuilder.Ld(__[HL], E);
                instructionsBuilder.Inc(HL);
                if (bytesToCopy == 2)
                {
                    instructionsBuilder.Ld(__[HL], D);
                    if (totalBytesToCopy > 0)
                    {
                        instructionsBuilder.Inc(HL);
                    }
                }

            } while (totalBytesToCopy > 0);
        }

        public static void CopyFromStackToIX(InstructionsBuilder instructionsBuilder, int size, int ixOffset = 0, bool restoreIX = false)
        {
            // TODO: When does it make sense to use LDIR instead??
            // e.g. if size > 255 then we'll have to emit code to alter IX so using ldir is probably better
            // suspect it may be much better for size > x where x is substantially less than 255 as the generated code will be quite large.
            // For small x should we ditch using ix completely and just use HL & DE e.g. LD (HL), D??

            int changeToIX = 0;

            var totalBytesToCopy = size;
            int originalIxOffset = ixOffset;

            do
            {
                var bytesToCopy = totalBytesToCopy > 2 ? 2 : totalBytesToCopy;

                // offset has to be -128 to + 127
                if (ixOffset + bytesToCopy > 128)
                {
                    // Need to move IX along to keep stackOffset within -128 to +127 range
                    short newIxChange = 0;
                    do
                    {
                        newIxChange += 127;
                        ixOffset -= 127;
                        size -= 127;
                    }
                    while (ixOffset + bytesToCopy > 128);

                    changeToIX += newIxChange;

                    instructionsBuilder.Ld(DE, newIxChange);
                    instructionsBuilder.Add(IX, DE);
                }

                while (ixOffset < -128)
                {
                    short newIxChange = 0;
                    do
                    {
                        newIxChange -= 128;
                        ixOffset += 128;
                        size += 128;
                    }
                    while (ixOffset < -128);

                    changeToIX += newIxChange;

                    instructionsBuilder.Ld(DE, newIxChange);
                    instructionsBuilder.Add(IX, DE);
                }

                instructionsBuilder.Pop(HL);
                if (bytesToCopy == 2)
                {
                    instructionsBuilder.Ld(__[IX + (short)(ixOffset + 1)], H);
                }
                instructionsBuilder.Ld(__[IX + (short)(ixOffset + 0)], L);

                ixOffset += 2;
                totalBytesToCopy -= 2;
            } while (ixOffset < size + originalIxOffset);

            if (changeToIX != 0 && restoreIX)
            {
                instructionsBuilder.Ld(DE, (short)(-changeToIX));
                instructionsBuilder.Add(IX, DE);
            }
        }

        public static void CopyFromIXToStack(InstructionsBuilder instructionsBuilder, int size, int ixOffset = 0, bool restoreIX = false)
        {
            int changeToIX = 0;

            int originalIxOffset = ixOffset;
            ixOffset += size - 2;
            do
            {
                var bytesToCopy = size > 2 ? 2 : size;
                size -= 2;

                // IX offset must lie between -128 to +127
                if (ixOffset < -128)
                {
                    // Move IX so new offset will be 126/127
                    var delta = -ixOffset + 126;
                    instructionsBuilder.Ld(DE, (short)-delta);
                    instructionsBuilder.Add(IX, DE);
                    changeToIX -= delta;

                    ixOffset += delta;
                    originalIxOffset += delta;
                }

                if (bytesToCopy == 1)
                {
                    instructionsBuilder.Ld(H, 0);
                    instructionsBuilder.Ld(L, __[IX + (short)(ixOffset + 1)]);
                }
                else
                {
                    instructionsBuilder.Ld(H, __[IX + (short)(ixOffset + 1)]);
                    instructionsBuilder.Ld(L, __[IX + (short)(ixOffset + 0)]);
                }
                instructionsBuilder.Push(HL);

                ixOffset -= 2;
            } while (ixOffset >= originalIxOffset);

            if (changeToIX != 0 && restoreIX)
            {
                instructionsBuilder.Ld(DE, (short)(-changeToIX));
                instructionsBuilder.Add(IX, DE);
            }
        }
    }
}
