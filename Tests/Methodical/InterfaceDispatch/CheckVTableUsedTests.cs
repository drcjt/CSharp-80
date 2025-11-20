namespace InterfaceDispatch
{
    public interface IFoo
    {
        string Who();
    }

    public class Base : IFoo
    {
        public virtual string Who() => "Base";
    }

    public class Derived : Base
    {
        public override string Who() => "Derived";
    }

    public class AnotherDerived : Base, IFoo
    {
        // Explicit interface implementation overrides the inherited virtual
        string IFoo.Who() => "AnotherDerived";
    }

    internal static class CheckVTableUsedTests
    {
        public static int RunTests()
        {
            IFoo f1 = new Base();
            IFoo f2 = new Derived();
            IFoo f3 = new AnotherDerived();

            // Expected behavior: dispatch should use the *runtime type’s* vtable slot
            if ("Base" != f1.Who()) return 1;
            if ("Derived" != f2.Who()) return 2;
            if ("AnotherDerived" != f3.Who()) return 3;

            return 0;
        }
    }
}
