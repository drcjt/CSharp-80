using Konamiman.Z80dotNet;
using NUnit.Framework;

namespace GenericsTests
{
    [TestFixture]
    public class GenericsTestsRunner
    {
        [Test]
        [TestCaseSource(typeof(GenericsTestsRunner), nameof(GenericsTestsCaseData))]
        public void GenericsTest(string testname)
        {
            RunTest(testname);
        }

        private static void RunTest(string assemblyFileName)
        {
            var z80 = new Z80Processor();

            // The Z80 simulator doesn't handle auto stop correctly
            // if the sp is manually manipulated e.g. ld sp, xx
            // so we have to disable it but will rely on auto stop
            // on halt
            z80.AutoStopOnRetWithStackEmpty = false;

            // read bytes from cim file and load into byte array                        
            var program = File.ReadAllBytes(assemblyFileName);

            z80.Memory.SetContents(0, program);

            //z80.BeforeInstructionExecution += Z80_BeforeInstructionExecution;

            z80.Start();

            Console.WriteLine($"Test {assemblyFileName} ran in {z80.TStatesElapsedSinceStart} T-states");

            // Validate we finished on the HALT instruction
            Assert.AreEqual(12, z80.Registers.PC);

            // Pass returns 32 bit 0 in DEHL
            Assert.AreEqual(0, z80.Registers.DE);
            Assert.AreEqual(0, z80.Registers.HL);
        }

        private static IEnumerable<TestCaseData> GenericsTestsCaseData
        {
            get
            {
                var genericsTestsPath = TestContext.CurrentContext.TestDirectory.RemoveDirectories(4);
                var binConfigTargetPath = TestContext.CurrentContext.TestDirectory.GetLastDirectories(3);
                string[] genericsTestsPaths = Directory.GetDirectories(genericsTestsPath);
                foreach (string genericsTestPath in genericsTestsPaths) 
                {
                    var genericsTestName = Path.GetFileName(genericsTestPath);
                    if (genericsTestName != nameof(GenericsTestsRunner))
                    {
                        var temp = Path.Combine(binConfigTargetPath, $"{genericsTestName}.cim");
                        var testAssemblyPath = Path.Combine(genericsTestPath, temp);

                        yield return new TestCaseData(testAssemblyPath).SetName(Path.GetFileNameWithoutExtension(genericsTestName));
                    }
                }
            }
        }                
    }
}