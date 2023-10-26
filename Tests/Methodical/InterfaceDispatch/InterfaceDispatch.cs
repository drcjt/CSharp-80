namespace InterfaceDispatch
{
    interface IA
    {
        int Add(int i);

        int Add2(int i);
    }

    interface IB
    {
        int Add(int i);
    }

    interface IC
    {
        int Add(int i);
    }

    interface ID : IA, IB { }

    class D : ID
    {
        int IA.Add(int i)
        {
            return 5;
        }

        int IA.Add2(int i)
        {
            return 6;
        }

        int IB.Add(int i)
        {
            return 7;
        }
    }

    public static class InterfaceDispachTests
    {
        private static int Test(ID d)
        {
            IA a = d;
            if (a.Add2(0) != 6)
                return 1;

            IB b = d;
            if (b.Add(0) != 7)
                return 1;

            return 0;
        }

        public static int Main()
        {
            D d = new D();

            if (Test(d) != 0)
                return 1;

            return 0;
        }
    }
}