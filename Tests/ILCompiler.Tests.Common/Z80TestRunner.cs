using Konamiman.Z80dotNet;
using NUnit.Framework;

namespace ILCompiler.Tests.Common
{
    public class Z80TestRunner
    {
        public static bool RunTest(string assemblyFileName, bool ilBvt = false)
        {
            var program = File.ReadAllBytes(assemblyFileName);
            RunTest(program, assemblyFileName, ilBvt);

            return true;
        }

        private static void RunTest(byte[]? z80Bytes, string testName, bool ilBvt)
        {
            var z80 = new Z80Processor();

            // The Z80 simulator doesn't handle auto stop correctly
            // if the sp is manually manipulated e.g. ld sp, xx
            // so we have to disable it but will rely on auto stop
            // on halt
            z80.AutoStopOnRetWithStackEmpty = false;

            z80.Memory.SetContents(0, z80Bytes);

            //z80.BeforeInstructionExecution += Z80_BeforeInstructionExecution;

            z80.Start();

            Console.WriteLine($"Test {testName} ran in {z80.TStatesElapsedSinceStart} T-states");

            // Validate we finished on the HALT instruction
            if (ilBvt)
            {
                Assert.AreEqual(19, z80.Registers.PC);
            }
            else
            {
                Assert.AreEqual(0x76, z80.Memory[z80.Registers.PC - 1]);
            }

            // Pass returns 32 bit 0 in DEHL
            Assert.AreEqual(0, z80.Registers.DE);
            Assert.AreEqual(0, z80.Registers.HL);

            if (ilBvt)
            {
                // Make sure stack pointer ends up at original place
                Assert.AreEqual(ILCompilerRunner.StackStart, (ushort)z80.Registers.SP);
            }
        }
    }
}