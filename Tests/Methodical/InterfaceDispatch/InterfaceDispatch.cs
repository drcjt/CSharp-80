namespace InterfaceDispatch
{
    public static class InterfaceDispachTests
    {
        public static int Main()
        {
            int result = 0;
            result = SimpleInterfaceTests.RunTests(); if (result != 0) return result;
            result = MultipleInterfaceTests.RunTests(); if (result != 0) return result;
            result = InheritanceInterfaceTests.RunTests(); if (result != 0) return result;

            return 0;
        }
    }
}