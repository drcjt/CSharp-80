namespace InterfaceDispatch
{
    interface IA
    {
        int Add(int i);
        int Add2(int i);
    }
    interface IB { int Add(int i); }
    interface IC { int Add(int i); }
    interface ID : IA, IB { }

    class D : ID
    {
        int IA.Add(int i) => 5;
        int IA.Add2(int i) => 6;
        int IB.Add(int i) => 7;
    }

    internal static class InheritanceInterfaceTests
    {
        private static int Test(ID d)
        {
            if (((IA)d).Add2(0) != 6)
                return 4;

            if (((IB)d).Add(0) != 7)
                return 5;

            return 0;
        }

        public static int RunTests() => Test(new D());
    }
}