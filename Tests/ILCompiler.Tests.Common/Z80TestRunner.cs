using Konamiman.Z80dotNet;
using NUnit.Framework;

namespace ILCompiler.Tests.Common
{
    public class Z80TestRunner
    {
        private string _solutionPath = "";
        private string _tracePath = "";
        public Z80TestRunner(string solutionPath)
        {
            _solutionPath = Path.GetFullPath(solutionPath);
        }

        public static Z80TestRunner Create(string solutionPath)
        {
            return new Z80TestRunner(solutionPath);
        }

        public bool RunTest(string assemblyFileName, bool ilBvt = false, bool benchmark = true)
        {
            _tracePath = Path.ChangeExtension(assemblyFileName, "ptc");
            var program = File.ReadAllBytes(assemblyFileName);
            RunTest(program, assemblyFileName, ilBvt, benchmark);

            return true;
        }

        private static byte[] GetNonZeroBytes()
        {
            var nonZeroBytes = new byte[UInt16.MaxValue];
            Array.Fill<byte>(nonZeroBytes, 0xff);

            return nonZeroBytes;
        }

        // Set to true to enable PC tracing to help debug failing tests
        private const bool TracePC = false;

        private Z80Processor z80 = new Z80Processor();
        private void RunTest(byte[]? z80Bytes, string testName, bool ilBvt, bool benchmark)
        {
            File.Delete(_tracePath);

            // The Z80 simulator doesn't handle auto stop correctly
            // if the sp is manually manipulated e.g. ld sp, xx
            // so we have to disable it but will rely on auto stop
            // on halt
            z80.AutoStopOnRetWithStackEmpty = false;


            // Start out with all memory set to 0xff
            // makes sure no tests happen to pass just because
            // memory contains zeros to begin with
            z80.Memory.SetContents(0, GetNonZeroBytes());

            // Load the program bytes
            z80.Memory.SetContents(0, z80Bytes);
            
            z80.BeforeInstructionExecution += Z80_BeforeInstructionExecution;

            z80.Start();

            Console.WriteLine($"Last value of PC = {z80.Registers.PC}");

            if (benchmark)
            {
                BenchmarkWriter bw = new BenchmarkWriter(_solutionPath);
                bw.WriteBenchmark(testName.Replace(_solutionPath, ""), z80.TStatesElapsedSinceStart);
            }

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

        private void Z80_BeforeInstructionExecution(object? sender, BeforeInstructionExecutionEventArgs e)
        {
            if (TracePC)
            {
                File.AppendAllText(_tracePath, $"0x{z80.Registers.PC:X}{Environment.NewLine}");
            }
        }
    }
}