namespace InvokeCctorByStaticMethod
{
    // This is beforefieldinit
    public class Measure
    {
        public static int a = 0xCC;
    }

    // This is not beforefieldinit
    public class TestClass
    {

        public static void f(ref byte b)
        {
            return;
        }

        static TestClass()
        {
            Measure.a = 8;
        }
    }

    public static class Test
    {
        public static int Main()
        {
            byte b = 0x0f;

            if (Measure.a != 0xCC)
            {
                return 1;
            }

            TestClass.f(ref b);

            if (Measure.a != 8)
            {
                return 1;
            }

            return 0;
        }
    }
}