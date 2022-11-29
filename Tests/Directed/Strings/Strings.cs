using System;

public static class AckermannTest
{
    public static int Main()
    {
        var testString = "Testing 1,2,3";

        var fifthCharacter = testString[4];

        if (fifthCharacter == 'i')
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }
}