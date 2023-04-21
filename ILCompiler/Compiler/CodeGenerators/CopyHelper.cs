using ILCompiler.Compiler.Emit;
using System.Diagnostics;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class CopyHelper
    {
        public static void CopyStackToSmall(Emitter emitter, int bytesToCopy, int ixOffset)
        {
            // pop lsw
            emitter.Pop(R16.HL);

            // pop msw and ignore it as for small data types we
            // truncate the value
            emitter.Pop(R16.DE);

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
                emitter.Ld(R16.DE, changeToIX);
                emitter.Add(I16.IX, R16.DE);
                ixOffset -= changeToIX;
            }

            if (bytesToCopy == 2)
            {
                emitter.Ld(I16.IX, (short)(ixOffset + 1), R8.H);
            }
            emitter.Ld(I16.IX, (short)(ixOffset + 0), R8.L);

            if (changeToIX != 0)
            {
                emitter.Ld(R16.DE, (short)-changeToIX);
                emitter.Add(I16.IX, R16.DE);
            }
        }

        public static void CopySmallToStack(Emitter emitter, int bytesToCopy, int ixOffset, bool signExtend)
        {
            Debug.Assert(bytesToCopy == 1 || bytesToCopy == 2);
            int changeToIX = 0;

            if (ixOffset + bytesToCopy < -127)
            {
                var delta = ixOffset + 1;
                emitter.Ld(R16.DE, (short)delta);
                emitter.Add(I16.IX, R16.DE);
                changeToIX += delta;

                ixOffset -= delta;
            }

            if (bytesToCopy == 1)
            {
                if (signExtend)
                {
                    emitter.Ld(R8.A, I16.IX, (short)(ixOffset));
                    emitter.Ld(R8.E, R8.A);

                    emitter.Add(R8.A, R8.A);
                    emitter.Sbc(R8.A, R8.A);
                    emitter.Ld(R8.H, R8.A);
                    emitter.Ld(R8.L, R8.A);
                    emitter.Push(R16.HL);

                    emitter.Ld(R8.L, R8.E);
                    emitter.Push(R16.HL);
                }
                else
                {
                    emitter.Ld(R16.HL, 0);
                    emitter.Push(R16.HL);

                    emitter.Ld(R8.H, 0);
                    emitter.Ld(R8.L, I16.IX, (short)(ixOffset));
                    emitter.Push(R16.HL);
                }
            }
            else
            {
                if (signExtend)
                {
                    emitter.Ld(R8.H, I16.IX, (short)(ixOffset + 1));
                    emitter.Ld(R8.L, I16.IX, (short)(ixOffset));

                    emitter.Ld(R8.D, R8.H);
                    emitter.Ld(R8.E, R8.L);

                    emitter.Add(R16.HL, R16.HL);  // move sign bit into carry flag
                    emitter.Sbc(R16.HL, R16.HL);  // hl is now 0000 or FFFF
                    emitter.Push(R16.HL);

                    emitter.Push(R16.DE);
                }
                else
                {
                    emitter.Ld(R16.HL, 0);
                    emitter.Push(R16.HL);

                    emitter.Ld(R8.H, I16.IX, (short)(ixOffset + 1));
                    emitter.Ld(R8.L, I16.IX, (short)(ixOffset));
                    emitter.Push(R16.HL);
                }
            }

            if (changeToIX != 0)
            {
                emitter.Ld(R16.DE, (short)(-changeToIX));
                emitter.Add(I16.IX, R16.DE);
            }
        }

        public static void CopyFromStackToIX(Emitter emitter, int size, int ixOffset = 0, bool restoreIX = false)
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

                    emitter.Ld(R16.DE, newIxChange);
                    emitter.Add(I16.IX, R16.DE);
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

                    emitter.Ld(R16.DE, newIxChange);
                    emitter.Add(I16.IX, R16.DE);
                }

                emitter.Pop(R16.HL);
                if (bytesToCopy == 2)
                {
                    emitter.Ld(I16.IX, (short)(ixOffset + 1), R8.H);
                }
                emitter.Ld(I16.IX, (short)(ixOffset + 0), R8.L);

                ixOffset += 2;
                totalBytesToCopy -= 2;
            } while (ixOffset < size + originalIxOffset);

            if (changeToIX != 0 && restoreIX)
            {
                emitter.Ld(R16.DE, (short)(-changeToIX));
                emitter.Add(I16.IX, R16.DE);
            }
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
                    emitter.Ld(R16.DE, (short)-delta);
                    emitter.Add(I16.IX, R16.DE);
                    changeToIX -= delta;

                    ixOffset += delta;
                    originalIxOffset += delta;
                }

                if (bytesToCopy == 1)
                {
                    emitter.Ld(R8.H, 0);
                    emitter.Ld(R8.L, I16.IX, (short)(ixOffset + 1));
                }
                else
                {
                    emitter.Ld(R8.H, I16.IX, (short)(ixOffset + 1));
                    emitter.Ld(R8.L, I16.IX, (short)(ixOffset + 0));
                }
                emitter.Push(R16.HL);

                ixOffset -= 2;
            } while (ixOffset >= originalIxOffset);

            if (changeToIX != 0 && restoreIX)
            {
                emitter.Ld(R16.DE, (short)(-changeToIX));
                emitter.Add(I16.IX, R16.DE);
            }
        }
    }
}
