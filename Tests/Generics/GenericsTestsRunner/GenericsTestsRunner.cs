using ILCompiler.Tests.Common;
using NUnit.Framework;

namespace GenericsTests
{
    [TestFixture]
    public class GenericsTestsRunner
    {
        [Test]
        [TestCaseSource(typeof(GenericsTestsRunner), nameof(GenericsTestsCaseData))]
        public void GenericsTest(string testName)
        {
            TestRunnerHelper.RunTest(testName, @".\..\..\..\..\..\..");
            Assert.Pass();
        }

        private static IEnumerable<TestCaseData> GenericsTestsCaseData => TestRunnerHelper.TestsCaseData();
    }
}