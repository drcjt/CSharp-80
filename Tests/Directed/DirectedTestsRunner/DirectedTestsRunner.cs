using ILCompiler.Tests.Common;
using NUnit.Framework;

namespace DirectedTests
{
    [TestFixture]
    public class DirectedTestsRunner
    {
        private const int StackStart = UInt16.MaxValue;

        [Test]
        [TestCaseSource(typeof(DirectedTestsRunner), nameof(DirectedTestsCaseData))]
        public void DirectedTest(string testname)
        {
            var isAssemblerTests = testname.EndsWith("Assembler.cim");
            ILCompilerRunner.Create(SolutionPath).CompileILAndAssemble(testname, outputCim: !isAssemblerTests, deleteAssembler: isAssemblerTests);
            if (!isAssemblerTests)
            {
                Z80TestRunner.Create(SolutionPath).RunTest(testname);
            }

            Assert.Pass();
        }

        private readonly string SolutionPath = Path.Combine(TestContext.CurrentContext.TestDirectory, @".\..\..\..\..\..\..");

        private static IEnumerable<TestCaseData> DirectedTestsCaseData
        {
            get
            {
                var directedTestsPath = TestContext.CurrentContext.TestDirectory.RemoveDirectories(4);
                var binConfigTargetPath = TestContext.CurrentContext.TestDirectory.GetLastDirectories(3);
                string[] directedTestsPaths = Directory.GetDirectories(directedTestsPath);
                foreach (string directedTestPath in directedTestsPaths) 
                {
                    var directedTestName = Path.GetFileName(directedTestPath);
                    if (directedTestName != nameof(DirectedTestsRunner))
                    {
                        var temp = Path.Combine(binConfigTargetPath, $"{directedTestName}.cim");
                        var testAssemblyPath = Path.Combine(directedTestPath, temp);

                        yield return new TestCaseData(testAssemblyPath).SetName(Path.GetFileNameWithoutExtension(directedTestName));
                    }
                }
            }
        }                
    }
}