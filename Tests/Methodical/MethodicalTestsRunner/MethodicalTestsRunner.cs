using ILCompiler.Tests.Common;
using NUnit.Framework;

namespace MethodicalTests
{
    [TestFixture]
    public class MethodicalTestsRunner
    {
        private const int StackStart = UInt16.MaxValue;

        [Test]
        [TestCaseSource(typeof(MethodicalTestsRunner), nameof(MethodicalTestsCaseData))]
        public void MethodicalTest(string testname)
        {
            // Determine if test is using il file or not
            var ilPath = Path.ChangeExtension(testname, ".il");
            if (File.Exists(ilPath))
            {
                // Assemble the il
                ILAsmRunner.Assemble(ilPath);

                // Run the test
                ILCompilerRunner.Create(SolutionPath).CompileILAndAssemble(testname, createLibrary : false);
                Z80TestRunner.Create(SolutionPath).RunTest(testname);
            }
            else
            {
                // No need to assemble any il as roslyn will have created assembled il
                ILCompilerRunner.Create(SolutionPath).CompileILAndAssemble(testname);
                Z80TestRunner.Create(SolutionPath).RunTest(testname);
            }

            Assert.Pass();
        }

        private readonly string SolutionPath = Path.Combine(TestContext.CurrentContext.TestDirectory, @".\..\..\..\..\..\..");

        private static IEnumerable<TestCaseData> MethodicalTestsCaseData
        {
            get
            {
                var methodicalTestsPath = TestContext.CurrentContext.TestDirectory.RemoveDirectories(4);
                var binConfigTargetPath = TestContext.CurrentContext.TestDirectory.GetLastDirectories(3);
                string[] methodicalTestsPaths = Directory.GetDirectories(methodicalTestsPath);
                foreach (string methodicalTestPath in methodicalTestsPaths) 
                {
                    var methodicalTestName = Path.GetFileName(methodicalTestPath);
                    if (methodicalTestName != nameof(MethodicalTestsRunner))
                    {
                        var temp = Path.Combine(binConfigTargetPath, $"{methodicalTestName}.cim");
                        var testAssemblyPath = Path.Combine(methodicalTestPath, temp);

                        yield return new TestCaseData(testAssemblyPath).SetName(Path.GetFileNameWithoutExtension(methodicalTestName));
                    }
                }
            }
        }                
    }
}