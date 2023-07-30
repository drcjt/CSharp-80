using System.Runtime.InteropServices;

namespace System.Drawing
{
    public enum Color
    {
        White = 0,
        Black = 1
    }

    public partial class Graphics
    {
        [DllImport(Libraries.Runtime, EntryPoint = "FASTSETRES")]
        public static extern void SetPixel(int x, int y, Color color = Color.White);

        // TODO: These are not part of standard MS Graphics class
        public const int ScreenWidth = 127;
        public const int ScreenHeight = 47;

        private unsafe static void SetRes(byte x, byte y, Color color = Color.White)
        {
            const ushort VideoRam = 0x3C00;

            // Work out video address containing the pixel to set or unset
            byte videoRow = (byte)(y / 3);
            byte* videoAddress = (byte*)VideoRam + (videoRow << 6) + (x >> 1);

            // Calculate the mask
            var mask = (y - videoRow * 3) switch
            {
                0 => 0x01,
                1 => 0x04,
                2 => 0x10,
                _ => 0x01,
            };
            if ((x & 1) == 1) mask <<= 1;   // Modify mask if right column

            // Get the current video byte from video ram
            var videoByte = *videoAddress;

            // If video byte is not graphic character then set to blank graphic character
            if (videoByte < 128) videoByte = 0x80;

            *videoAddress = color switch
            {
                Color.White => (byte)(videoByte | mask),
                Color.Black => (byte)(videoByte & ~mask),
                _ => videoByte,
            };
        }
    }
}
