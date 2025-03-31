using ILCompiler.Tests.Common;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace BenchmarkTests
{
    [TestFixture]
    public class BenchmarkTestsRunner
    {
        [Test]
        [TestCaseSource(typeof(BenchmarkTestsRunner), nameof(BenchmarkTestsCaseData))]
        public void MethodicalTest(string testname)
        {
            TestRunnerHelper.RunTest(testname, @".\..\..\..\..\..\..\..\..");
            Assert.Pass();
        }

        private static IEnumerable<TestCaseData> BenchmarkTestsCaseData => TestRunnerHelper.TestsCaseData();
    }
}