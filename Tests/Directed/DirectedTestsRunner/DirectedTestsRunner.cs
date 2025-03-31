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
            TestRunnerHelper.RunTest(testname, @".\..\..\..\..\..\..");
            Assert.Pass();
        }

        private static IEnumerable<TestCaseData> DirectedTestsCaseData => TestRunnerHelper.TestsCaseData();
    }
}