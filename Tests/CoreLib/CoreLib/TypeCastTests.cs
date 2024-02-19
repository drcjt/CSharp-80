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
    }
}
