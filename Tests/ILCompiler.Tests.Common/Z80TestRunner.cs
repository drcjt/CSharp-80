using Konamiman.Z80dotNet;
using NUnit.Framework;
using System.Text;

namespace ILCompiler.Tests.Common
{
    public class Z80TestRunner
    {
        private readonly string _solutionPath;
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

        private readonly Z80Processor z80 = new Z80Processor();
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
            const int orgAddress = 0x0050;

            z80.Memory.SetContents(orgAddress, z80Bytes);

            // Setup "Jp x050" as initial instruction - note that z80.Start
            // sets the PC to 0
            z80.Memory.SetContents(0, [0xC3, 0x50, 0x00]);

            // Setup print char routine to just return
            z80.Memory.SetContents(0x33, [0xc9]);

            z80.BeforeInstructionFetch += BeforeInstructionFetch;

            var startHeap = BitConverter.ToUInt16(z80.Memory.GetContents(orgAddress + 1, 2), 0);

            z80.Start();

            const int heapNextAddress = 0x0065;
            var endHeap = BitConverter.ToUInt16(z80.Memory.GetContents(heapNextAddress, 2), 0);
            var bytesAllocated = endHeap - startHeap;

            Console.WriteLine(_consoleStringBuilder.ToString());

            Console.WriteLine($"Last value of PC = {z80.Registers.PC}");

            if (benchmark)
            {
                BenchmarkWriter bw = new BenchmarkWriter(_solutionPath);
                bw.WriteBenchmark(testName.Replace(_solutionPath, ""), z80.TStatesElapsedSinceStart);
            }

            Console.WriteLine($"Test {testName} ran in {z80.TStatesElapsedSinceStart} T-states, allocated {bytesAllocated} bytes");

            // Validate we finished on the HALT instruction
            if (ilBvt)
            {
                Assert.That(z80.Registers.PC, Is.EqualTo(19 + orgAddress));
            }
            else
            {
                Assert.That(z80.Memory[z80.Registers.PC - 1], Is.EqualTo(0x76));
            }

            // Pass returns 32 bit 0 in DEHL
            Assert.That(z80.Registers.DE, Is.EqualTo(0));
            Assert.That(z80.Registers.HL, Is.EqualTo(0));

            if (ilBvt)
            {
                // Make sure stack pointer ends up at original place
                Assert.That((ushort)z80.Registers.SP, Is.EqualTo(ILCompilerRunner.StackStart));
            }
        }

        private readonly StringBuilder _consoleStringBuilder = new StringBuilder();

        private void BeforeInstructionFetch(object? sender, BeforeInstructionFetchEventArgs e)
        {
            if (z80.Registers.PC == 0x33)
            {
                // Print char
                _consoleStringBuilder.Append((char)(z80.Registers.A));
            }

            if (TracePC)
            {
                File.AppendAllText(_tracePath, $"0x{z80.Registers.PC:X}{Environment.NewLine}");
            }
        }
    }
}
