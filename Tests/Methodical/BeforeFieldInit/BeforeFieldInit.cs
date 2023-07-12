namespace StaticFieldInit
{
    public static class TestClass
    {
        public static int retVal = 0;
        public static int TestMethod(bool cond)
        {
            if (cond)
            {
                return OtherClass.X;
            }

            return 0;
        }
    }

    public class OtherClass
    {
        public static readonly int X = GetX();

        static int GetX()
        {
            TestClass.retVal = 1;
            return 1;
        }
    }

    public static class Test
    {
        public static int Main()
        {
            TestClass.TestMethod(false);
            return TestClass.retVal;
        }
    }
}