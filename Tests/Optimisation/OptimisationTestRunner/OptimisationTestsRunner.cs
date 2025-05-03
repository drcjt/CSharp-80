using ILCompiler.Tests.Common;
using NUnit.Framework;

namespace OptimisationTests
{
    [TestFixture]
    public class OptimisationTestsRunner
    {
        [Test]
        [TestCaseSource(typeof(OptimisationTestsRunner), nameof(OptimisationTestsCaseData))]
        public void OptimisationTest(string testName)
        {
            TestRunnerHelper.RunTest(testName, @".\..\..\..\..\..\..");
            Assert.Pass();
        }

        private static IEnumerable<TestCaseData> OptimisationTestsCaseData => TestRunnerHelper.TestsCaseData();
    }
}