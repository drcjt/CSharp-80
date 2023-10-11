namespace VirtualMethods
{
    public class BaseClass
    {
        public virtual int F1() => 1;
        public virtual int F2() => 20;
    }

    public class  DerivedClass : BaseClass
    {
        public override int F1() => base.F1() + 1;
        public new virtual int F2() => 30;
    }

    public static class Test
    {
        public static int Main()
        {
            var baseClass = new BaseClass();
            if (baseClass.F1() != 1)
            {
                return 1;
            }
            if (baseClass.F2() != 20)
            {
                return 1;
            }
            
            var derivedClass = new DerivedClass();
            if (derivedClass.F1() != 2) 
            {
                return 1;
            }
            if (derivedClass.F2() != 30)
            {
                return 1;
            }

            return 0;
        }
    }
}