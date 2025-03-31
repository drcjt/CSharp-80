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
            TestRunnerHelper.RunTest(testname, @".\..\..\..\..\..\..");
            Assert.Pass();
        }

        private static IEnumerable<TestCaseData> CodeGenTestsCaseData => TestRunnerHelper.TestsCaseData();
    }
}