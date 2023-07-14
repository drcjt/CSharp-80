namespace InvokeCctorByInstanceMethod
{
    // This is beforefieldinit
    public class Measure
    {
        public static int a = 0xCC;
    }

    // This is not beforefieldinit
    public class TestClass
    {
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

            // This should trigger the cctor as ctor is an instance method
            TestClass t = new TestClass();

            if (Measure.a != 8)
            {
                return 1;
            }

            return 0;
        }
    }
}