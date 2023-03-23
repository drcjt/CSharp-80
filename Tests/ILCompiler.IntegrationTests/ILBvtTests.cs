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
            ILAsmRunner.Assemble(ilFileName);
            ILCompilerRunner.Create(SolutionPath).CompileILAndAssemble(ilFileName, createLibrary: false);

            var cimFileName = Path.Combine(TestContext.CurrentContext.TestDirectory, ".\\il_bvt\\" + NUnit.Framework.TestContext.CurrentContext.Test.Name + ".cim");
            Z80TestRunner.RunTest(cimFileName, true);
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

                    yield return new TestCaseData(targetFilePath).SetName(Path.GetFileNameWithoutExtension(targetFilePath));
                }
            }
        }
    }
}
