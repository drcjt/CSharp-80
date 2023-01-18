namespace System.Drawing
{
    public sealed class Pens
    {
        private static Pen s_white = null;
        public static Pen White
        {
            get
            {
                if (s_white == null)
                {
                    s_white = new Pen(Color.White);
                }
                return s_white;
            }
        }

        private static Pen s_black = null;
        public static Pen Black
        {
            get
            {
                if (s_black == null)
                {
                    s_black = new Pen(Color.Black);
                }
                return s_black;
            }
        }
    }
}
