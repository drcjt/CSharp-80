namespace Cctor
{
    public class TestClass
    {
        public static int a = 0xCC;
    }

    public static class Test
    {
        public static int Main()
        {
            if (TestClass.a != 0xCC)
            {
                return 1;
            }

            TestClass.a = 8;

            if (TestClass.a != 8)
            {
                return 1;
            }

            return 0;
        }
    }
}