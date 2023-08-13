using ILCompiler.Tests.Common;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace MethodicalTests
{
    [TestFixture]
    public class MethodicalTestsRunner
    {
        private const string IlExtension = ".il";
        private const string CimExtension = ".cim";

        private const int StackStart = UInt16.MaxValue;

        [Test]
        [TestCaseSource(typeof(MethodicalTestsRunner), nameof(MethodicalTestsCaseData))]
        public void MethodicalTest(string testname)
        {
            // Determine if test is using il file or not
            if (Path.GetExtension(testname) == IlExtension)
            {
                // Assemble the il
                if (!ILAsmRunner.Assemble(testname))
                {
                    Assert.Fail("Failed to assemble IL");
                }

                var cimFile = Path.ChangeExtension(testname, CimExtension);

                // Run the test
                ILCompilerRunner.Create(SolutionPath).CompileILAndAssemble(cimFile, createLibrary : false);
                Z80TestRunner.Create(SolutionPath).RunTest(cimFile);
            }
            else
            {
                // No need to assemble any il as roslyn will have created assembled il
                ILCompilerRunner.Create(SolutionPath).CompileILAndAssemble(testname);
                Z80TestRunner.Create(SolutionPath).RunTest(testname);
            }

            Assert.Pass();
        }

        private readonly string SolutionPath = Path.Combine(TestContext.CurrentContext.TestDirectory, @".\..\..\..\..\..\..");

        private static IEnumerable<TestCaseData> MethodicalTestsCaseData
        {
            get
            {
                var methodicalTestsPath = TestContext.CurrentContext.TestDirectory.RemoveDirectories(4);
                var binConfigTargetPath = TestContext.CurrentContext.TestDirectory.GetLastDirectories(3);
                string[] methodicalTestsPaths = Directory.GetDirectories(methodicalTestsPath);
                foreach (string methodicalTestPath in methodicalTestsPaths) 
                {
                    var methodicalTestName = Path.GetFileName(methodicalTestPath);
                    if (methodicalTestName != nameof(MethodicalTestsRunner))
                    {
                        var ilFiles = Directory.GetFiles(methodicalTestPath, "*.il");

                        if (ilFiles.Length == 0)
                        {

                            var temp = Path.Combine(binConfigTargetPath, $"{methodicalTestName}{CimExtension}");
                            var testAssemblyPath = Path.Combine(methodicalTestPath, temp);

                            yield return new TestCaseData(testAssemblyPath).SetName(methodicalTestName);
                        }
                        else
                        {
                            foreach (var ilFile in ilFiles)
                            {
                                var ilFileName = Path.GetFileName(ilFile);
                                var temp = Path.Combine(binConfigTargetPath, ilFileName);
                                var testAssemblyPath = Path.Combine(methodicalTestPath, temp);

                                var testName = $"{methodicalTestName} ({Path.GetFileNameWithoutExtension(ilFile)})";

                                yield return new TestCaseData(testAssemblyPath).SetName(testName);
                            }
                        }
                    }
                }
            }
        }                
    }
}