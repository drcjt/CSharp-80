using ILCompiler.Common.TypeSystem.Common;
using NUnit.Framework;

namespace ILCompiler.UnitTests
{
    [TestFixture]
    internal class FieldLayoutTests
    {
        [Test]
        public void TestLayoutIntAlignUp()
        {
            var target = new TargetDetails(TargetArchitecture.Z80);

            Assert.AreEqual(new LayoutInt(0), LayoutInt.AlignUp(new LayoutInt(0), new LayoutInt(1), target));
            Assert.AreEqual(new LayoutInt(0), LayoutInt.AlignUp(new LayoutInt(0), new LayoutInt(2), target));
            Assert.AreEqual(new LayoutInt(0), LayoutInt.AlignUp(new LayoutInt(0), new LayoutInt(4), target));

            Assert.AreEqual(new LayoutInt(1), LayoutInt.AlignUp(new LayoutInt(1), new LayoutInt(1), target));
            Assert.AreEqual(new LayoutInt(2), LayoutInt.AlignUp(new LayoutInt(2), new LayoutInt(1), target));
            Assert.AreEqual(new LayoutInt(3), LayoutInt.AlignUp(new LayoutInt(3), new LayoutInt(1), target));
            Assert.AreEqual(new LayoutInt(4), LayoutInt.AlignUp(new LayoutInt(4), new LayoutInt(1), target));
            Assert.AreEqual(new LayoutInt(5), LayoutInt.AlignUp(new LayoutInt(5), new LayoutInt(1), target));
            Assert.AreEqual(new LayoutInt(6), LayoutInt.AlignUp(new LayoutInt(6), new LayoutInt(1), target));
            Assert.AreEqual(new LayoutInt(7), LayoutInt.AlignUp(new LayoutInt(7), new LayoutInt(1), target));
            Assert.AreEqual(new LayoutInt(8), LayoutInt.AlignUp(new LayoutInt(8), new LayoutInt(1), target));
            Assert.AreEqual(new LayoutInt(9), LayoutInt.AlignUp(new LayoutInt(9), new LayoutInt(1), target));
            Assert.AreEqual(new LayoutInt(10), LayoutInt.AlignUp(new LayoutInt(10), new LayoutInt(1), target));
            Assert.AreEqual(new LayoutInt(11), LayoutInt.AlignUp(new LayoutInt(11), new LayoutInt(1), target));
            Assert.AreEqual(new LayoutInt(12), LayoutInt.AlignUp(new LayoutInt(12), new LayoutInt(1), target));
            Assert.AreEqual(new LayoutInt(13), LayoutInt.AlignUp(new LayoutInt(13), new LayoutInt(1), target));
            Assert.AreEqual(new LayoutInt(14), LayoutInt.AlignUp(new LayoutInt(14), new LayoutInt(1), target));
            Assert.AreEqual(new LayoutInt(15), LayoutInt.AlignUp(new LayoutInt(15), new LayoutInt(1), target));
            Assert.AreEqual(new LayoutInt(16), LayoutInt.AlignUp(new LayoutInt(16), new LayoutInt(1), target));

            Assert.AreEqual(new LayoutInt(2), LayoutInt.AlignUp(new LayoutInt(1), new LayoutInt(2), target));
            Assert.AreEqual(new LayoutInt(2), LayoutInt.AlignUp(new LayoutInt(2), new LayoutInt(2), target));
            Assert.AreEqual(new LayoutInt(4), LayoutInt.AlignUp(new LayoutInt(3), new LayoutInt(2), target));
            Assert.AreEqual(new LayoutInt(4), LayoutInt.AlignUp(new LayoutInt(4), new LayoutInt(2), target));
            Assert.AreEqual(new LayoutInt(6), LayoutInt.AlignUp(new LayoutInt(5), new LayoutInt(2), target));
            Assert.AreEqual(new LayoutInt(6), LayoutInt.AlignUp(new LayoutInt(6), new LayoutInt(2), target));
            Assert.AreEqual(new LayoutInt(8), LayoutInt.AlignUp(new LayoutInt(7), new LayoutInt(2), target));
            Assert.AreEqual(new LayoutInt(8), LayoutInt.AlignUp(new LayoutInt(8), new LayoutInt(2), target));
            Assert.AreEqual(new LayoutInt(10), LayoutInt.AlignUp(new LayoutInt(9), new LayoutInt(2), target));
            Assert.AreEqual(new LayoutInt(10), LayoutInt.AlignUp(new LayoutInt(10), new LayoutInt(2), target));
            Assert.AreEqual(new LayoutInt(12), LayoutInt.AlignUp(new LayoutInt(11), new LayoutInt(2), target));
            Assert.AreEqual(new LayoutInt(12), LayoutInt.AlignUp(new LayoutInt(12), new LayoutInt(2), target));
            Assert.AreEqual(new LayoutInt(14), LayoutInt.AlignUp(new LayoutInt(13), new LayoutInt(2), target));
            Assert.AreEqual(new LayoutInt(14), LayoutInt.AlignUp(new LayoutInt(14), new LayoutInt(2), target));
            Assert.AreEqual(new LayoutInt(16), LayoutInt.AlignUp(new LayoutInt(15), new LayoutInt(2), target));
            Assert.AreEqual(new LayoutInt(16), LayoutInt.AlignUp(new LayoutInt(16), new LayoutInt(2), target));

            Assert.AreEqual(new LayoutInt(4), LayoutInt.AlignUp(new LayoutInt(1), new LayoutInt(4), target));
            Assert.AreEqual(new LayoutInt(4), LayoutInt.AlignUp(new LayoutInt(2), new LayoutInt(4), target));
            Assert.AreEqual(new LayoutInt(4), LayoutInt.AlignUp(new LayoutInt(3), new LayoutInt(4), target));
            Assert.AreEqual(new LayoutInt(4), LayoutInt.AlignUp(new LayoutInt(4), new LayoutInt(4), target));
            Assert.AreEqual(new LayoutInt(8), LayoutInt.AlignUp(new LayoutInt(5), new LayoutInt(4), target));
            Assert.AreEqual(new LayoutInt(8), LayoutInt.AlignUp(new LayoutInt(6), new LayoutInt(4), target));
            Assert.AreEqual(new LayoutInt(8), LayoutInt.AlignUp(new LayoutInt(7), new LayoutInt(4), target));
            Assert.AreEqual(new LayoutInt(8), LayoutInt.AlignUp(new LayoutInt(8), new LayoutInt(4), target));
            Assert.AreEqual(new LayoutInt(12), LayoutInt.AlignUp(new LayoutInt(9), new LayoutInt(4), target));
            Assert.AreEqual(new LayoutInt(12), LayoutInt.AlignUp(new LayoutInt(10), new LayoutInt(4), target));
            Assert.AreEqual(new LayoutInt(12), LayoutInt.AlignUp(new LayoutInt(11), new LayoutInt(4), target));
            Assert.AreEqual(new LayoutInt(12), LayoutInt.AlignUp(new LayoutInt(12), new LayoutInt(4), target));
            Assert.AreEqual(new LayoutInt(16), LayoutInt.AlignUp(new LayoutInt(13), new LayoutInt(4), target));
            Assert.AreEqual(new LayoutInt(16), LayoutInt.AlignUp(new LayoutInt(14), new LayoutInt(4), target));
            Assert.AreEqual(new LayoutInt(16), LayoutInt.AlignUp(new LayoutInt(15), new LayoutInt(4), target));
            Assert.AreEqual(new LayoutInt(16), LayoutInt.AlignUp(new LayoutInt(16), new LayoutInt(4), target));
        }
    }
}