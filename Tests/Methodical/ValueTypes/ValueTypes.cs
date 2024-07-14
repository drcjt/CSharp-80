using System;

namespace ValueTypes
{
    public static class Test
    {
        public static int Main()
        {
            int i = 25;
            object o = i;

            if (i.ToString() != "25")
                return 1;
            
            if (((Int32)o).ToString() != "25")
                return 2;

            if (o.ToString() != "25")
                return 3;

            return 0;
        }
    }
}