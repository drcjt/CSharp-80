using ILCompiler.Tests.Common;
using NUnit.Framework;

namespace CodeGenTests
{
    [TestFixture]
    public class CodeGenTestsRunner
    {
        [Test]
        [TestCaseSource(typeof(CodeGenTestsRunner), nameof(CodeGenTestsCaseData))]
        public void CodeGenTest(string testname)
        {
            ILCompilerRunner.Create(SolutionPath).CompileILAndAssemble(testname);
            Z80TestRunner.Create(SolutionPath).RunTest(testname, false, false);

            Assert.Pass();
        }

        private readonly string SolutionPath = Path.Combine(TestContext.CurrentContext.TestDirectory, @".\..\..\..\..\..\..");

        private static IEnumerable<TestCaseData> CodeGenTestsCaseData
        {
            get
            {
                var codeGenTestsPath = TestContext.CurrentContext.TestDirectory.RemoveDirectories(4);
                var binConfigTargetPath = TestContext.CurrentContext.TestDirectory.GetLastDirectories(3);
                string[] codeGenTestsPaths = Directory.GetDirectories(codeGenTestsPath);
                foreach (string codeGenTestPath in codeGenTestsPaths) 
                {
                    var codeGenTestName = Path.GetFileName(codeGenTestPath);
                    if (codeGenTestName != nameof(CodeGenTestsRunner))
                    {
                        var temp = Path.Combine(binConfigTargetPath, $"{codeGenTestName}.cim");
                        var testAssemblyPath = Path.Combine(codeGenTestPath, temp);

                        // Visual Studio doesn't like .'s in test names so replace with a character that looks like a dot but isn't
                        var testName = Path.GetFileNameWithoutExtension(codeGenTestName);
                        testName = testName.Replace('.', '\u2024');

                        yield return new TestCaseData(testAssemblyPath).SetName(testName);
                    }
                }
            }
        }                
    }
}