using ILCompiler.Tests.Common;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace CSharp80.Tests.BVT
{
    [TestFixture]
    public class ILBvtTests
    {
        [Test]
        [TestCaseSource(typeof(ILBvtTests), nameof(IlBvtTestCaseData))]
        public void IlBvtTest(string ilFileName)
        {
            if (!ILAsmRunner.Assemble(ilFileName))
            {
                Assert.Fail("Failed to assemble IL");
            }
            ILCompilerRunner.Create(SolutionPath).CompileILAndAssemble(ilFileName, createLibrary: false);

            var cimFileName = Path.Combine(TestContext.CurrentContext.TestDirectory, ".\\il_bvt\\" + NUnit.Framework.TestContext.CurrentContext.Test.Name + ".cim");
            Z80TestRunner.Create(SolutionPath).RunTest(cimFileName, true);
            Assert.Pass();
        }

        private readonly string SolutionPath = Path.Combine(TestContext.CurrentContext.TestDirectory, @".\..\..\..\..\..");

        private static IEnumerable<TestCaseData> IlBvtTestCaseData
        {
            get
            {
                var ilFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, @".\..\..\..\il_bvt");
                var files = Directory.GetFiles(ilFilePath, "*.il");

                if (!Directory.Exists(@".\il_bvt"))
                {
                    Directory.CreateDirectory(@".\il_bvt");
                }

                foreach (var file in files)
                {
                    var targetFilePath = Path.Combine(@".\il_bvt", Path.GetFileName(file));
                    File.Copy(file, targetFilePath, true);

                    // Visual Studio doesn't like .'s in test names so replace with a character that looks like a dot but isn't
                    var testName = Path.GetFileNameWithoutExtension(targetFilePath);
                    testName = testName.Replace('.', '\u2024');

                    yield return new TestCaseData(targetFilePath).SetName(testName);
                }
            }
        }
    }
}
