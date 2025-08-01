﻿using ILCompiler.Compiler.Emit;
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
            if (ixOffset > 127 || ixOffset < -128)
            {
                // Make sure IX offset is within -128 to +127 range
                changeToIX = (short)ixOffset;
                instructionsBuilder.Ld(DE, changeToIX);
                instructionsBuilder.Add(IX, DE);
                ixOffset = 0;
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
            CodeGeneratorHelper.AddHLFromDE(instructionsBuilder, (short)offset);

            instructionsBuilder.Pop(DE);
            instructionsBuilder.Pop(BC);    // Ignore msw

            instructionsBuilder.Ld(__[HL], E);

            if (bytesToCopy == 2)
            {
                instructionsBuilder.Inc(HL);
                instructionsBuilder.Ld(__[HL], D);
            }
        }

        public static void CopySmallFromHLToStack(InstructionsBuilder instructionsBuilder, int bytesToCopy, int offset, bool signExtend)
        {
            // Assume pointer to copy from is in HL
            Debug.Assert(bytesToCopy == 1 || bytesToCopy == 2);

            var delta = offset + bytesToCopy - 1;
            CodeGeneratorHelper.AddHLFromDE(instructionsBuilder, (short)delta);

            if (!signExtend)
            {
                instructionsBuilder.Ld(DE, 0);
                instructionsBuilder.Push(DE);

                if (bytesToCopy == 1)
                {
                    instructionsBuilder.Ld(D, 0);
                }
                else
                {
                    instructionsBuilder.Ld(D, __[HL]);
                    instructionsBuilder.Dec(HL);
                }
                instructionsBuilder.Ld(E, __[HL]);
                instructionsBuilder.Push(DE);
            }
            else if (bytesToCopy == 1)
            {
                instructionsBuilder.Ld(A, __[HL]);
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
                instructionsBuilder.Ld(B, __[HL]);
                instructionsBuilder.Dec(HL);
                instructionsBuilder.Ld(C, __[HL]);

                instructionsBuilder.Ld(H, B);
                instructionsBuilder.Ld(L, C);

                instructionsBuilder.Add(HL, HL);  // move sign bit into carry flag
                instructionsBuilder.Sbc(HL, HL);  // hl is now 0000 or FFFF
                instructionsBuilder.Push(HL);

                instructionsBuilder.Push(BC);
            }
        }

        public static void CopySmallFromIXToStack(InstructionsBuilder instructionsBuilder, int bytesToCopy, int ixOffset, bool signExtend)
        {
            Debug.Assert(bytesToCopy == 1 || bytesToCopy == 2);
            int changeToIX = 0;

            if (ixOffset > 127 || ixOffset < -128)
            {
                // Make sure IX offset is within -128 to +127 range
                changeToIX = (short)ixOffset;
                instructionsBuilder.Ld(BC, (short)changeToIX);
                instructionsBuilder.Add(IX, BC);
                ixOffset = 0;
            }

            if (!signExtend)
            {
                instructionsBuilder.Ld(HL, 0);
                instructionsBuilder.Push(HL);

                if (bytesToCopy == 1)
                {
                    instructionsBuilder.Ld(H, 0);
                }
                else
                {
                    instructionsBuilder.Ld(H, __[IX + (short)(ixOffset + 1)]);
                }
                instructionsBuilder.Ld(L, __[IX + (short)(ixOffset)]);
                instructionsBuilder.Push(HL);
            }
            else if (bytesToCopy == 1)
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
                instructionsBuilder.Ld(H, __[IX + (short)(ixOffset + 1)]);
                instructionsBuilder.Ld(L, __[IX + (short)(ixOffset)]);

                instructionsBuilder.Ld(D, H);
                instructionsBuilder.Ld(E, L);

                instructionsBuilder.Add(HL, HL);  // move sign bit into carry flag
                instructionsBuilder.Sbc(HL, HL);  // hl is now 0000 or FFFF
                instructionsBuilder.Push(HL);

                instructionsBuilder.Push(DE);
            }

            if (changeToIX != 0)
            {
                instructionsBuilder.Ld(BC, (short)(-changeToIX));
                instructionsBuilder.Add(IX, BC);
            }
        }

        /*
         *  Assumes address to copy to is in HL
         */
        public static void CopyFromStackToHL(InstructionsBuilder instructionsBuilder, int size, int offset = 0)
        {
            CodeGeneratorHelper.AddHLFromDE(instructionsBuilder, (short)offset);

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

            var registerHigh = H;
            var registerLow = L;

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

                    instructionsBuilder.Ld(BC, newIxChange);
                    instructionsBuilder.Add(IX, BC);
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

                    instructionsBuilder.Ld(BC, newIxChange);
                    instructionsBuilder.Add(IX, BC);
                }

                if (registerHigh == D) instructionsBuilder.Pop(DE);
                if (registerHigh == H) instructionsBuilder.Pop(HL);
                if (bytesToCopy == 2)
                {
                    instructionsBuilder.Ld(__[IX + (short)(ixOffset + 1)], registerHigh);
                }
                instructionsBuilder.Ld(__[IX + (short)(ixOffset + 0)], registerLow);

                registerHigh = D;
                registerLow = E;

                ixOffset += 2;
                totalBytesToCopy -= 2;
            } while (ixOffset < size + originalIxOffset);

            if (changeToIX != 0 && restoreIX)
            {
                instructionsBuilder.Ld(BC, (short)(-changeToIX));
                instructionsBuilder.Add(IX, BC);
            }
        }

        public static void CopyFromHLToStack(InstructionsBuilder instructionsBuilder, int size, int offset = 0)
        {
            // Assume address is in HL

            // Add offset to HL            
            var delta = offset + size - 1;
            CodeGeneratorHelper.AddHLFromDE(instructionsBuilder, (short)delta);

            // Copy from HL to stack
            do
            {
                if (size > 1)
                {
                    instructionsBuilder.Ld(B, __[HL]);
                    instructionsBuilder.Dec(HL);
                }
                else
                {
                    instructionsBuilder.Ld(B, 0);
                }
                instructionsBuilder.Ld(C, __[HL]);

                if (size - 2 > 0)
                {
                    instructionsBuilder.Dec(HL);
                }

                instructionsBuilder.Push(BC);

                size -= 2;
            }
            while (size > 0);
        }

        public static void CopyFromIXToStack(InstructionsBuilder instructionsBuilder, int size, int ixOffset = 0, bool restoreIX = false)
        {
            int changeToIX = 0;

            bool pushImmediately = size > 4;
            int originalSize = size;
            var destHigh = size == 2 ? H : D;
            var destLow = size == 2 ? L : E;

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
                    instructionsBuilder.Ld(BC, (short)-delta);
                    instructionsBuilder.Add(IX, BC);
                    changeToIX -= delta;

                    ixOffset += delta;
                    originalIxOffset += delta;
                }

                if (bytesToCopy == 1)
                {
                    instructionsBuilder.Ld(destHigh, 0);
                    instructionsBuilder.Ld(destLow, __[IX + (short)(ixOffset + 1)]);
                }
                else
                {
                    instructionsBuilder.Ld(destHigh, __[IX + (short)(ixOffset + 1)]);
                    instructionsBuilder.Ld(destLow, __[IX + (short)(ixOffset + 0)]);
                }

                if (pushImmediately)
                {
                    if (destHigh == H) instructionsBuilder.Push(HL);
                    if (destHigh == D) instructionsBuilder.Push(DE);
                }

                destHigh = H;
                destLow = L;

                ixOffset -= 2;
            } while (ixOffset >= originalIxOffset);

            if (!pushImmediately)
            {
                if (originalSize == 4)
                {
                    instructionsBuilder.Push(DE);
                }
                instructionsBuilder.Push(HL);
            }

            if (changeToIX != 0 && restoreIX)
            {
                instructionsBuilder.Ld(BC, (short)(-changeToIX));
                instructionsBuilder.Add(IX, BC);
            }
        }
    }
}
