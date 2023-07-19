using System.Runtime.InteropServices;

namespace System.Drawing
{
    public enum Color
    {
        White = 0,
        Black = 12,     // Corresponds to INVERSE 1
    }

    public partial class Graphics
    {
        [DllImport(Libraries.Runtime, EntryPoint = "SETRES")]
        public static extern void SetPixel(int x, int y, Color color = Color.White);

        // TODO: These are not part of standard MS Graphics class
        public const int ScreenWidth = 255;
        public const int ScreenHeight = 175;    // Doesn't include bottom 2 lines
    }
}