using ILCompiler.Tests.Common;
using NUnit.Framework;

namespace RegressionTests
{
    [TestFixture]
    public class RegressionTestsRunner
    {
        private const int StackStart = UInt16.MaxValue;

        [Test]
        [TestCaseSource(typeof(RegressionTestsRunner), nameof(RegressionTestsCaseData))]
        public void RegressionTest(string testname)
        {
            ILCompilerRunner.Create(SolutionPath).CompileILAndAssemble(testname);
            Z80TestRunner.Create(SolutionPath).RunTest(testname, false, false);
            Assert.Pass();
        }

        private readonly string SolutionPath = Path.Combine(TestContext.CurrentContext.TestDirectory, @".\..\..\..\..\..\..");

        private static IEnumerable<TestCaseData> RegressionTestsCaseData
        {
            get
            {
                var regressionTestsPath = TestContext.CurrentContext.TestDirectory.RemoveDirectories(4);
                var binConfigTargetPath = TestContext.CurrentContext.TestDirectory.GetLastDirectories(3);
                string[] regressionTestsPaths = Directory.GetDirectories(regressionTestsPath);
                foreach (string regressionTestPath in regressionTestsPaths) 
                {
                    var regressionTestName = Path.GetFileName(regressionTestPath);
                    if (regressionTestName != nameof(RegressionTestsRunner))
                    {
                        var temp = Path.Combine(binConfigTargetPath, $"{regressionTestName}.cim");
                        var testAssemblyPath = Path.Combine(regressionTestPath, temp);

                        yield return new TestCaseData(testAssemblyPath).SetName(Path.GetFileNameWithoutExtension(regressionTestName));
                    }
                }
            }
        }                
    }
}