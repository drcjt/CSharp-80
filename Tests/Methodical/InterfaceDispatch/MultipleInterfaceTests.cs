namespace InterfaceDispatch
{
    interface IInterfaceOfInt
    {
        int GetInt();
    }

    class TestClassOfInt : IMyInterface, IInterfaceOfInt
    {
        public int GetAnInt() => 1;
        public int GetInt() => 5;
    }

    internal static class MultipleInterfaceTests
    {
        public static int RunTests()
        {
            TestClassOfInt testInt = new();

            if (((IMyInterface)testInt).GetAnInt() != 1) return 2;
            if (((IInterfaceOfInt)testInt).GetInt() != 5) return 3;

            return 0;
        }
    }
}
