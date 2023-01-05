public static class Regression
{
    public static int Main()
    {
        var result = 0;

        result = Bug87();
        if (result != 0) return 87;

        return 0;
    }

    public static int Bug87()
    {
        char[] test = new char[1] { 'a' };
        Bug87_Method(test[0]);

        return 0;
    }

    private static void Bug87_Method(char ch)
    {
    }
}