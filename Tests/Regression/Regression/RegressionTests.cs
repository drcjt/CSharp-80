public static class Regression
{
    public static int Main()
    {
        Bug87();

        return 0;
    }

    public static void Bug87()
    {
        char[] test = new char[1] { 'a' };
        Bug87_Method(test[0]);
    }

    private static void Bug87_Method(char ch)
    {
    }
}