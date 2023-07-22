using ILCompiler.Compiler.Emit;
using System.Diagnostics;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal static class CopyHelper
    {
        /*
         * Assumes address to copy to is in HL
         */
        public static void CopyStackToSmall(Emitter emitter, int bytesToCopy, int offset)
        {
            if (offset != 0)
            {
                emitter.Ld(DE, (short)offset);
                emitter.Add(HL, DE);
            }

            emitter.Pop(DE);
            emitter.Pop(BC);    // Ignore msw

            emitter.Ld(__[HL], E);
            emitter.Inc(HL);

            if (bytesToCopy == 2)
            {
                emitter.Ld(__[HL], D);
            }
        }

        public static void CopySmallToStack(Emitter emitter, int bytesToCopy, int ixOffset, bool signExtend)
        {
            Debug.Assert(bytesToCopy == 1 || bytesToCopy == 2);
            int changeToIX = 0;

            if (ixOffset + bytesToCopy < -127)
            {
                var delta = ixOffset + 1;
                emitter.Ld(DE, (short)delta);
                emitter.Add(IX, DE);
                changeToIX += delta;

                ixOffset -= delta;
            }

            if (bytesToCopy == 1)
            {
                if (signExtend)
                {
                    emitter.Ld(A, __[IX + (short)(ixOffset)]);
                    emitter.Ld(E, A);

                    emitter.Add(A, A);
                    emitter.Sbc(A, A);
                    emitter.Ld(H, A);
                    emitter.Ld(L, A);
                    emitter.Push(HL);

                    emitter.Ld(L, E);
                    emitter.Push(HL);
                }
                else
                {
                    emitter.Ld(HL, 0);
                    emitter.Push(HL);

                    emitter.Ld(H, 0);
                    emitter.Ld(L, __[IX + (short)(ixOffset)]);
                    emitter.Push(HL);
                }
            }
            else
            {
                if (signExtend)
                {
                    emitter.Ld(H, __[IX + (short)(ixOffset + 1)]);
                    emitter.Ld(L, __[IX + (short)(ixOffset)]);

                    emitter.Ld(D, H);
                    emitter.Ld(E, L);

                    emitter.Add(HL, HL);  // move sign bit into carry flag
                    emitter.Sbc(HL, HL);  // hl is now 0000 or FFFF
                    emitter.Push(HL);

                    emitter.Push(DE);
                }
                else
                {
                    emitter.Ld(HL, 0);
                    emitter.Push(HL);

                    emitter.Ld(H, __[IX + (short)(ixOffset + 1)]);
                    emitter.Ld(L, __[IX + (short)(ixOffset)]);
                    emitter.Push(HL);
                }
            }

            if (changeToIX != 0)
            {
                emitter.Ld(DE, (short)(-changeToIX));
                emitter.Add(IX, DE);
            }
        }

        /*
         *  Assumes address to copy to is in HL
         */
        public static void CopyFromStackToHL(Emitter emitter, int size, int offset = 0)
        {
            if (offset != 0)
            {
                emitter.Ld(DE, (short)offset);
                emitter.Add(HL, DE);
            }

            var totalBytesToCopy = size;
            do
            {
                var bytesToCopy = totalBytesToCopy > 2 ? 2 : totalBytesToCopy;
                totalBytesToCopy -= 2;

                emitter.Pop(DE);

                emitter.Ld(__[HL], E);
                emitter.Inc(HL);
                if (bytesToCopy == 2)
                {
                    emitter.Ld(__[HL], D);
                    if (totalBytesToCopy > 0)
                    {
                        emitter.Inc(HL);
                    }
                }

            } while (totalBytesToCopy > 0);
        }

        public static void CopyFromIXToStack(Emitter emitter, int size, int ixOffset = 0, bool restoreIX = false)
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
                    emitter.Ld(DE, (short)-delta);
                    emitter.Add(IX, DE);
                    changeToIX -= delta;

                    ixOffset += delta;
                    originalIxOffset += delta;
                }

                if (bytesToCopy == 1)
                {
                    emitter.Ld(H, 0);
                    emitter.Ld(L, __[IX + (short)(ixOffset + 1)]);
                }
                else
                {
                    emitter.Ld(H, __[IX + (short)(ixOffset + 1)]);
                    emitter.Ld(L, __[IX + (short)(ixOffset + 0)]);
                }
                emitter.Push(HL);

                ixOffset -= 2;
            } while (ixOffset >= originalIxOffset);

            if (changeToIX != 0 && restoreIX)
            {
                emitter.Ld(DE, (short)(-changeToIX));
                emitter.Add(IX, DE);
            }
        }
    }
}
