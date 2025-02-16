using ILCompiler.Tests.Common;
using NUnit.Framework;

namespace DirectedTests
{
    [TestFixture]
    public class DirectedTestsRunner
    {
        [Test]
        [TestCaseSource(typeof(DirectedTestsRunner), nameof(DirectedTestsCaseData))]
        public void DirectedTest(string testname)
        {
            ILCompilerRunner.Create(SolutionPath).CompileILAndAssemble(testname);
            Z80TestRunner.Create(SolutionPath).RunTest(testname);

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

                        // Visual Studio doesn't like .'s in test names so replace with a character that looks like a dot but isn't
                        var testName = Path.GetFileNameWithoutExtension(directedTestName);
                        testName = testName.Replace('.', '\u2024');

                        yield return new TestCaseData(testAssemblyPath).SetName(testName);
                    }
                }
            }
        }                
    }
}