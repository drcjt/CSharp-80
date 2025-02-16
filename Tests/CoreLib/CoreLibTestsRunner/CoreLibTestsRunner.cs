using ILCompiler.Tests.Common;
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
            ILCompilerRunner.Create(SolutionPath).CompileILAndAssemble(testname);
            Z80TestRunner.Create(SolutionPath).RunTest(testname, false, false);

            Assert.Pass();
        }

        private readonly string SolutionPath = Path.Combine(TestContext.CurrentContext.TestDirectory, @".\..\..\..\..\..\..");

        private static IEnumerable<TestCaseData> CoreLibTestsCaseData
        {
            get
            {
                var coreLibTestsPath = TestContext.CurrentContext.TestDirectory.RemoveDirectories(4);
                var binConfigTargetPath = TestContext.CurrentContext.TestDirectory.GetLastDirectories(3);
                string[] coreLibTestsPaths = Directory.GetDirectories(coreLibTestsPath);
                foreach (string coreLibTestPath in coreLibTestsPaths) 
                {
                    if (Directory.GetFiles(coreLibTestPath, "*.csproj").Length > 0)
                    {
                        var coreLibTestName = Path.GetFileName(coreLibTestPath);
                        if (coreLibTestName != nameof(CoreLibTestsRunner))
                        {
                            var temp = Path.Combine(binConfigTargetPath, $"{coreLibTestName}.cim");
                            var testAssemblyPath = Path.Combine(coreLibTestPath, temp);

                            // Visual Studio doesn't like .'s in test names so replace with a character that looks like a dot but isn't
                            coreLibTestName = coreLibTestName.Replace('.', '\u2024');

                            yield return new TestCaseData(testAssemblyPath).SetName(coreLibTestName);
                        }
                    }
                }
            }
        }                
    }
}