using ILCompiler.TypeSystem.Common;
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

            Assert.That(LayoutInt.AlignUp(new LayoutInt(0), new LayoutInt(1), target), Is.EqualTo(new LayoutInt(0)));

            Assert.That(LayoutInt.AlignUp(new LayoutInt(0), new LayoutInt(2), target), Is.EqualTo(new LayoutInt(0)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(0), new LayoutInt(4), target), Is.EqualTo(new LayoutInt(0)));

            Assert.That(LayoutInt.AlignUp(new LayoutInt(1), new LayoutInt(1), target), Is.EqualTo(new LayoutInt(1)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(2), new LayoutInt(1), target), Is.EqualTo(new LayoutInt(2)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(3), new LayoutInt(1), target), Is.EqualTo(new LayoutInt(3)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(4), new LayoutInt(1), target), Is.EqualTo(new LayoutInt(4)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(5), new LayoutInt(1), target), Is.EqualTo(new LayoutInt(5)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(6), new LayoutInt(1), target), Is.EqualTo(new LayoutInt(6)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(7), new LayoutInt(1), target), Is.EqualTo(new LayoutInt(7)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(8), new LayoutInt(1), target), Is.EqualTo(new LayoutInt(8)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(9), new LayoutInt(1), target), Is.EqualTo(new LayoutInt(9)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(10), new LayoutInt(1), target), Is.EqualTo(new LayoutInt(10)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(11), new LayoutInt(1), target), Is.EqualTo(new LayoutInt(11)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(12), new LayoutInt(1), target), Is.EqualTo(new LayoutInt(12)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(13), new LayoutInt(1), target), Is.EqualTo(new LayoutInt(13)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(14), new LayoutInt(1), target), Is.EqualTo(new LayoutInt(14)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(15), new LayoutInt(1), target), Is.EqualTo(new LayoutInt(15)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(16), new LayoutInt(1), target), Is.EqualTo(new LayoutInt(16)));

            Assert.That(LayoutInt.AlignUp(new LayoutInt(1), new LayoutInt(2), target), Is.EqualTo(new LayoutInt(2)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(2), new LayoutInt(2), target), Is.EqualTo(new LayoutInt(2)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(3), new LayoutInt(2), target), Is.EqualTo(new LayoutInt(4)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(4), new LayoutInt(2), target), Is.EqualTo(new LayoutInt(4)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(5), new LayoutInt(2), target), Is.EqualTo(new LayoutInt(6)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(6), new LayoutInt(2), target), Is.EqualTo(new LayoutInt(6)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(7), new LayoutInt(2), target), Is.EqualTo(new LayoutInt(8)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(8), new LayoutInt(2), target), Is.EqualTo(new LayoutInt(8)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(9), new LayoutInt(2), target), Is.EqualTo(new LayoutInt(10)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(10), new LayoutInt(2), target), Is.EqualTo(new LayoutInt(10)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(11), new LayoutInt(2), target), Is.EqualTo(new LayoutInt(12)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(12), new LayoutInt(2), target), Is.EqualTo(new LayoutInt(12)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(13), new LayoutInt(2), target), Is.EqualTo(new LayoutInt(14)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(14), new LayoutInt(2), target), Is.EqualTo(new LayoutInt(14)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(15), new LayoutInt(2), target), Is.EqualTo(new LayoutInt(16)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(16), new LayoutInt(2), target), Is.EqualTo(new LayoutInt(16)));

            Assert.That(LayoutInt.AlignUp(new LayoutInt(1), new LayoutInt(4), target), Is.EqualTo(new LayoutInt(4)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(2), new LayoutInt(4), target), Is.EqualTo(new LayoutInt(4)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(3), new LayoutInt(4), target), Is.EqualTo(new LayoutInt(4)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(4), new LayoutInt(4), target), Is.EqualTo(new LayoutInt(4)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(5), new LayoutInt(4), target), Is.EqualTo(new LayoutInt(8)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(6), new LayoutInt(4), target), Is.EqualTo(new LayoutInt(8)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(7), new LayoutInt(4), target), Is.EqualTo(new LayoutInt(8)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(8), new LayoutInt(4), target), Is.EqualTo(new LayoutInt(8)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(9), new LayoutInt(4), target), Is.EqualTo(new LayoutInt(12)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(10), new LayoutInt(4), target), Is.EqualTo(new LayoutInt(12)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(11), new LayoutInt(4), target), Is.EqualTo(new LayoutInt(12)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(12), new LayoutInt(4), target), Is.EqualTo(new LayoutInt(12)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(13), new LayoutInt(4), target), Is.EqualTo(new LayoutInt(16)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(14), new LayoutInt(4), target), Is.EqualTo(new LayoutInt(16)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(15), new LayoutInt(4), target), Is.EqualTo(new LayoutInt(16)));
            Assert.That(LayoutInt.AlignUp(new LayoutInt(16), new LayoutInt(4), target), Is.EqualTo(new LayoutInt(16)));
        }
    }
}