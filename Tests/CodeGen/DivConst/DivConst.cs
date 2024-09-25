namespace DivConst
{
    static class DivConst
    {
        public static int I4_Div_1(int i4) => i4 / 1;
        public static int I4_Div_Minus1(int i4) => i4 / -1;
        public static int I4_DivPow2_2(int i4) => i4 / 2;
    }

    public static class Program
    {
        public static int Main()
        {
            const int Pass = 0;
            const int Fail = -1;

            if (DivConst.I4_Div_1(42) != 42)
            {
                return Fail;
            }

            if (DivConst.I4_Div_Minus1(42) != -42)
            {
                return Fail;
            }

            if (DivConst.I4_DivPow2_2(42) != 21)
            {
                return Fail;
            }

            if (DivConst.I4_DivPow2_2(43) != 21)
            {
                return Fail;
            }

            if (DivConst.I4_DivPow2_2(-42) != -21)
            {
                return Fail;
            }

            if (DivConst.I4_DivPow2_2(-43) != -21)
            {
                return Fail;
            }

            return Pass;
        }
    }
}