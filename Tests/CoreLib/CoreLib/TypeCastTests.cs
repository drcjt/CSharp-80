using System;
using Xunit;

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

            Assert.True(sub is SubClass);
            Assert.True(sub is SuperClass);
            Assert.True(super is SuperClass);
            Assert.False(super is SubClass);
            Assert.True(super is Object);
            Assert.True(sub is Object);
        }

        public static void InterfaceTypeCastTests()
        {
            var one = new One();
            var two = new Two();
            var oneAndTwo = new OneAndTwo();

            Assert.True(one is IOne);
            Assert.False(one is ITwo);

            Assert.False(two is IOne);
            Assert.True(two is ITwo);

            Assert.True(oneAndTwo is IOne);
            Assert.True(oneAndTwo is ITwo);
        }
    }
}
