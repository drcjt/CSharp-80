using System.Runtime.InteropServices;


namespace System
{
    public static partial class Environment
    {
        public static int TickCount => 0;

        public static DateTime GetDateTime() { return new DateTime(); }

        public static char NewLine => '\r';
    }
}