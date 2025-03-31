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
            TestRunnerHelper.RunTest(testname, @".\..\..\..\..\..\..");
            Assert.Pass();
        }

        private static IEnumerable<TestCaseData> CoreLibTestsCaseData => TestRunnerHelper.TestsCaseData();
    }
}