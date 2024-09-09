using dnlib.DotNet;
using System;

namespace CoreLib
{
    internal class SuperClass
    {
        public SuperClass() { }
    }

    internal class  SubClass : SuperClass
    {
        public SubClass() : base() { }
    }

    internal interface IOne
    {
    }

    internal interface ITwo
    {

    }

    internal class One : IOne
    {
    }

    internal class Two : ITwo
    {
    }

    internal class OneAndTwo : IOne, ITwo
    {
    }

    internal class TypeCastTests
    {
        public static void ClassTypeCastTests()
        {
            var super = new SuperClass();
            var sub = new SubClass();

            Assert.IsTrue(sub is SubClass);
            Assert.IsTrue(sub is SuperClass);
            Assert.IsTrue(super is SuperClass);
            Assert.IsFalse(super is SubClass);
            Assert.IsTrue(super is Object);
            Assert.IsTrue(sub is Object);
        }

        public static void InterfaceTypeCastTests()
        {
            var one = new One();
            var two = new Two();
            var oneAndTwo = new OneAndTwo();

            Assert.IsTrue(one is IOne);
            Assert.IsFalse(one is ITwo);

            Assert.IsFalse(two is IOne);
            Assert.IsTrue(two is ITwo);

            Assert.IsTrue(oneAndTwo is IOne);
            Assert.IsTrue(oneAndTwo is ITwo);
        }
    }
}
