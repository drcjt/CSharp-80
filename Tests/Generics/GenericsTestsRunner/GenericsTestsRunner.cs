using Konamiman.Z80dotNet;
using NUnit.Framework;
using System.Diagnostics;
using System.Reflection;

namespace GenericsTests
{
    [TestFixture]
    public class GenericsTestsRunner
    {
        private const int StackStart = UInt16.MaxValue;

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
            Assert.AreEqual(0x76, z80.Memory[z80.Registers.PC - 1]);

            // Pass returns 32 bit 0 in DEHL
            Assert.AreEqual(0, z80.Registers.DE);
            Assert.AreEqual(0, z80.Registers.HL);
        }

        private readonly string SolutionPath = Path.Combine(TestContext.CurrentContext.TestDirectory, @".\..\..\..\..\..\..");

        private void Zmac(string ilFileName)
        {
            var asmFileName = Path.ChangeExtension(ilFileName, "asm");
            var cimFileName = Path.ChangeExtension(ilFileName, "cim");

            var zmacPath = Path.Combine(SolutionPath, @".\tools\zmac.exe");

            var arguments = $"--oo cim -o {cimFileName} {asmFileName}";

            RunProcess(zmacPath, arguments);
        }

        private void CompileIL(string ilFileName)
        {
            var currentType = MethodBase.GetCurrentMethod().DeclaringType;
            var assemblyConfigurationAttribute = currentType.Assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
            var buildConfigurationName = assemblyConfigurationAttribute?.Configuration;

            var asmFileName = Path.ChangeExtension(ilFileName, "asm");
            var exeFileName = Path.ChangeExtension(ilFileName, "dll");

            var corelibPath = Path.Combine(SolutionPath, $@".\System.Private.CoreLib\bin\{buildConfigurationName}\net7.0\System.Private.CoreLib.dll");

            var arguments = $"--ignoreUnknownCil false --printReturnCode false --integrationTests true --corelibPath {corelibPath} --outputFile {asmFileName} {exeFileName} --stackStart {StackStart}";
            var compiled = ILCompiler.Program.Main(arguments.Split(' '));

            Assert.AreEqual(0, compiled, "IL Failed to compile");
        }

        private bool RunProcess(string filename, string arguments)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = filename;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    var output = process.StandardOutput.ReadToEnd();
                    var errors = process.StandardError.ReadToEnd();

                    Console.WriteLine($"Process failed");
                    Console.WriteLine(output);
                    Console.WriteLine(errors);

                    return false;
                }
            }

            return true;
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