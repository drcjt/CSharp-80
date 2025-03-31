using ILCompiler.Tests.Common;
using NUnit.Framework;

namespace RegressionTests
{
    [TestFixture]
    public class RegressionTestsRunner
    {
        [Test]
        [TestCaseSource(typeof(RegressionTestsRunner), nameof(RegressionTestsCaseData))]
        public void RegressionTest(string testname)
        {
            TestRunnerHelper.RunTest(testname, @".\..\..\..\..\..\..");
            Assert.Pass();
        }

        private static IEnumerable<TestCaseData> RegressionTestsCaseData => TestRunnerHelper.TestsCaseData();
    }
}