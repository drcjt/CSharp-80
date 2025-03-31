using ILCompiler.Tests.Common;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace MethodicalTests
{
    [TestFixture]
    public class MethodicalTestsRunner
    {
        [Test]
        [TestCaseSource(typeof(MethodicalTestsRunner), nameof(MethodicalTestsCaseData))]
        public void MethodicalTest(string testName)
        {
            TestRunnerHelper.RunTest(testName, @".\..\..\..\..\..\..");
            Assert.Pass();
        }

        private static IEnumerable<TestCaseData> MethodicalTestsCaseData => TestRunnerHelper.TestsCaseData();
    }
}