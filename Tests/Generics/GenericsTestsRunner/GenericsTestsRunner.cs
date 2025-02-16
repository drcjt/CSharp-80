using ILCompiler.Tests.Common;
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
            ILCompilerRunner.Create(SolutionPath).CompileILAndAssemble(testname);
            Z80TestRunner.Create(SolutionPath).RunTest(testname);
            Assert.Pass();
        }

        private readonly string SolutionPath = Path.Combine(TestContext.CurrentContext.TestDirectory, @".\..\..\..\..\..\..");

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

                        // Visual Studio doesn't like .'s in test names so replace with a character that looks like a dot but isn't
                        var testName = Path.GetFileNameWithoutExtension(genericsTestName);
                        testName = testName.Replace('.', '\u2024');

                        yield return new TestCaseData(testAssemblyPath).SetName(testName);
                    }
                }
            }
        }                
    }
}