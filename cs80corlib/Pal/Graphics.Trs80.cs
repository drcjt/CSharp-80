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
        [DllImport(Libraries.Runtime, EntryPoint = "SETRES")]
        public static extern void SetPixel(int x, int y, Color color = Color.White);
    }
}
