using System.Runtime.InteropServices;

namespace System.Drawing
{
    public enum Color
    {
        White = 0x80,
        Black = 1
    }

    public partial class Graphics
    {
        //[DllImport(Libraries.Runtime, EntryPoint = "SETRES")]
        public static void SetPixel(int x, int y, Color color = Color.White) {  }

        // TODO: These are not part of standard MS Graphics class
        public const int ScreenWidth = 249;
        public const int ScreenHeight = 191;
    }
}