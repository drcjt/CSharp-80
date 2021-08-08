using Konamiman.Z80dotNet;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
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
            var z80 = new Z80Processor();
            z80.AutoStopOnRetWithStackEmpty = true;

            // read bytes from cim file and load into byte array
            var program = File.ReadAllBytes(ilFileName);

            z80.Memory.SetContents(0, program);

            z80.Start();

            // Pass returns 32 bit 0 in DEHL
            Assert.AreEqual(0, z80.Registers.DE);
            Assert.AreEqual(0, z80.Registers.HL);
        }

        private static IEnumerable<TestCaseData> IlBvtTestCaseData
        {
            get
            {
                var files = Directory.GetFiles(@".\il_bvt", "*.cim");

                foreach (var file in files)
                {
                    yield return new TestCaseData(file).SetName(Path.GetFileNameWithoutExtension(file));
                }
            }
        }
    }
}
