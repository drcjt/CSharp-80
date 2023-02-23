using Konamiman.Z80dotNet;
using NUnit.Framework;

namespace CoreLibTests
{
    [TestFixture]
    public class CoreLibTestsRunner
    {
        [Test]
        [TestCaseSource(typeof(CoreLibTestsRunner), nameof(CoreLibTestsCaseData))]
        public void CoreLibTest(string testname)
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
            Assert.AreEqual(0x76, z80.Memory[z80.Registers.PC - 1]);

            // Pass returns 32 bit 0 in DEHL
            Assert.AreEqual(0, z80.Registers.DE);
            Assert.AreEqual(0, z80.Registers.HL);
        }

        private static IEnumerable<TestCaseData> CoreLibTestsCaseData
        {
            get
            {
                var coreLibTestsPath = TestContext.CurrentContext.TestDirectory.RemoveDirectories(4);
                var binConfigTargetPath = TestContext.CurrentContext.TestDirectory.GetLastDirectories(3);
                string[] coreLibTestsPaths = Directory.GetDirectories(coreLibTestsPath);
                foreach (string coreLibTestPath in coreLibTestsPaths) 
                {
                    var coreLibTestName = Path.GetFileName(coreLibTestPath);
                    if (coreLibTestName != nameof(CoreLibTestsRunner))
                    {
                        var temp = Path.Combine(binConfigTargetPath, $"{coreLibTestName}.cim");
                        var testAssemblyPath = Path.Combine(coreLibTestPath, temp);

                        yield return new TestCaseData(testAssemblyPath).SetName(Path.GetFileNameWithoutExtension(coreLibTestName));
                    }
                }
            }
        }                
    }
}