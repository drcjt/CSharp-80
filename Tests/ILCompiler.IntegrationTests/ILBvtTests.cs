using Konamiman.Z80dotNet;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace CSharp80.Tests.BVT
{
    [TestFixture]
    public class ILBvtTests
    {
        private const int StackStart = UInt16.MaxValue;

        [Test]
        [TestCaseSource(typeof(ILBvtTests), nameof(IlBvtTestCaseData))]
        public void IlBvtTest(string ilFileName)
        {
            var ilFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, ilFileName);
            Assemble(ilFileName);
            CompileIL(ilFileName);
            Zmac(ilFileName);

            RunTest();
        }

        private static void RunTest()
        {
            var z80 = new Z80Processor();

            // The Z80 simulator doesn't handle auto stop correctly
            // if the sp is manually manipulated e.g. ld sp, xx
            // so we have to disable it but will rely on auto stop
            // on halt
            z80.AutoStopOnRetWithStackEmpty = false;

            // read bytes from cim file and load into byte array
            var ilFileName = Path.Combine(TestContext.CurrentContext.TestDirectory, ".\\il_bvt\\" + NUnit.Framework.TestContext.CurrentContext.Test.Name + ".cim");
            var program = File.ReadAllBytes(ilFileName);

            z80.Memory.SetContents(0, program);

            z80.Start();

            // Validate we finished on the HALT instruction
            Assert.AreEqual(19, z80.Registers.PC);

            // Pass returns 32 bit 0 in DEHL
            Assert.AreEqual(0, z80.Registers.DE);
            Assert.AreEqual(0, z80.Registers.HL);

            // Make sure stack pointer ends up at original place
            Assert.AreEqual(StackStart, (ushort)z80.Registers.SP);

        }

        private readonly string SolutionPath = Path.Combine(TestContext.CurrentContext.TestDirectory, @".\..\..\..\..\..");

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
            var exeFileName = Path.ChangeExtension(ilFileName, "exe");

            var corelibPath = Path.Combine(SolutionPath, $@".\System.Private.CoreLib\bin\{buildConfigurationName}\net7.0\System.Private.CoreLib.dll");

            var arguments = $"--ignoreUnknownCil false --printReturnCode false --integrationTests true --corelibPath {corelibPath} --outputFile {asmFileName} {exeFileName} --stackStart {StackStart}";
            var compiled = ILCompiler.Program.Main(arguments.Split(' '));

            Assert.AreEqual(0, compiled, "IL Failed to compile");
        }

        private void Assemble(string ilFileName)
        {
            var ilAsmPath = @"%USERPROFILE%\.nuget\packages\microsoft.netcore.ilasm\6.0.0\runtimes\native\ilasm.exe";
            ilAsmPath = Environment.ExpandEnvironmentVariables(ilAsmPath);

            RunProcess(ilAsmPath, ilFileName);
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


        private static IEnumerable<TestCaseData> IlBvtTestCaseData
        {
            get
            {
                var ilFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, @".\..\..\..\il_bvt");
                var files = Directory.GetFiles(ilFilePath, "*.il");

                if (!Directory.Exists(@".\il_bvt"))
                {
                    Directory.CreateDirectory(@".\il_bvt");
                }

                foreach (var file in files)
                {
                    var targetFilePath = Path.Combine(@".\il_bvt", Path.GetFileName(file));
                    File.Copy(file, targetFilePath, true);

                    yield return new TestCaseData(targetFilePath).SetName(Path.GetFileNameWithoutExtension(targetFilePath));
                }
            }
        }
    }
}
