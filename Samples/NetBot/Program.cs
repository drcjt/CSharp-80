using System.Drawing;

namespace MiniBCL
{
    public static class Program
    {
        public static void Main()
        {
            DrawDotNetBot();
        }

        private static void DrawDotNetBot(int y = 10)
        {
            var whitePen = Pens.White;
            Graphics.SetPixel(12, y + 1);
            Graphics.DrawLine(whitePen, 11, y + 2, 13, y + 2);
            Graphics.DrawLine(whitePen, 12, y + 3, 13, y + 3);
            Graphics.DrawLine(whitePen, 12, y + 4, 13, y + 4);
            Graphics.DrawLine(whitePen, 12, y + 5, 13, y + 5);
            Graphics.DrawLine(whitePen, 27, y + 5, 28, y + 5);
            Graphics.DrawLine(whitePen, 10, y + 6, 17, y + 6);
            Graphics.DrawLine(whitePen, 26, y + 6, 29, y + 6);
            Graphics.DrawLine(whitePen, 8, y + 7, 19, y + 7);
            Graphics.DrawLine(whitePen, 24, y + 7, 29, y + 7);
            Graphics.DrawLine(whitePen, 7, y + 8, 20, y + 8);
            Graphics.DrawLine(whitePen, 24, y + 8, 29, y + 8);
            Graphics.DrawLine(whitePen, 6, y + 9, 21, y + 9);
            Graphics.DrawLine(whitePen, 25, y + 9, 29, y + 9);
            Graphics.DrawLine(whitePen, 6, y + 10, 22, y + 10);
            Graphics.DrawLine(whitePen, 26, y + 10, 28, y + 10);
            Graphics.DrawLine(whitePen, 5, y + 11, 22, y + 11);
            Graphics.DrawLine(whitePen, 27, y + 11, 28, y + 11);
            Graphics.DrawLine(whitePen, 5, y + 12, 23, y + 12);
            Graphics.DrawLine(whitePen, 27, y + 12, 28, y + 12);
            Graphics.DrawLine(whitePen, 4, y + 13, 23, y + 13);
            Graphics.SetPixel(27, y + 13);
            Graphics.DrawLine(whitePen, 4, y + 14, 11, y + 14);
            Graphics.DrawLine(whitePen, 26, y + 14, 27, y + 14);
            Graphics.DrawLine(whitePen, 4, y + 15, 11, y + 15);
            Graphics.SetPixel(15, y + 15);
            Graphics.SetPixel(21, y + 15);
            Graphics.DrawLine(whitePen, 25, y + 15, 26, y + 15);
            Graphics.DrawLine(whitePen, 4, y + 16, 11, y + 16);
            Graphics.SetPixel(15, y + 16);
            Graphics.SetPixel(21, y + 16);
            Graphics.DrawLine(whitePen, 23, y + 16, 25, y + 16);
            Graphics.DrawLine(whitePen, 4, y + 17, 11, y + 17);
            Graphics.DrawLine(whitePen, 18, y + 17, 20, y + 17);
            Graphics.DrawLine(whitePen, 23, y + 17, 24, y + 17);
            Graphics.DrawLine(whitePen, 3, y + 18, 23, y + 18);
            Graphics.DrawLine(whitePen, 2, y + 19, 22, y + 19);
            Graphics.DrawLine(whitePen, 2, y + 20, 3, y + 20);
            Graphics.DrawLine(whitePen, 6, y + 20, 16, y + 20);
            Graphics.DrawLine(whitePen, 21, y + 20, 22, y + 20);
            Graphics.SetPixel(2, y + 21);
            Graphics.SetPixel(18, y + 21);
            Graphics.SetPixel(21, y + 21);
            Graphics.DrawLine(whitePen, 1, y + 22, 2, y + 22);
            Graphics.DrawLine(whitePen, 7, y + 22, 16, y + 22);
            Graphics.SetPixel(21, y + 22);
            Graphics.DrawLine(whitePen, 1, y + 23, 2, y + 23);
            Graphics.DrawLine(whitePen, 8, y + 23, 20, y + 23);
            Graphics.DrawLine(whitePen, 2, y + 24, 5, y + 24);
            Graphics.DrawLine(whitePen, 10, y + 24, 17, y + 24);
            Graphics.DrawLine(whitePen, 1, y + 25, 6, y + 25);
            Graphics.DrawLine(whitePen, 11, y + 25, 12, y + 25);
            Graphics.DrawLine(whitePen, 16, y + 25, 17, y + 25);
            Graphics.DrawLine(whitePen, 1, y + 26, 4, y + 26);
            Graphics.SetPixel(11, y + 26);
            Graphics.DrawLine(whitePen, 16, y + 26, 17, y + 26);
            Graphics.DrawLine(whitePen, 1, y + 27, 5, y + 27);
            Graphics.DrawLine(whitePen, 10, y + 27, 11, y + 27);
            Graphics.DrawLine(whitePen, 16, y + 27, 17, y + 27);
            Graphics.DrawLine(whitePen, 2, y + 28, 4, y + 28);
            Graphics.DrawLine(whitePen, 10, y + 28, 11, y + 28);
            Graphics.SetPixel(17, y + 28);
            Graphics.SetPixel(4, y + 29);
            Graphics.DrawLine(whitePen, 10, y + 29, 11, y + 29);
            Graphics.SetPixel(17, y + 29);
            Graphics.DrawLine(whitePen, 10, y + 30, 11, y + 30);
            Graphics.DrawLine(whitePen, 16, y + 30, 18, y + 30);
            Graphics.DrawLine(whitePen, 10, y + 31, 12, y + 31);
            Graphics.DrawLine(whitePen, 16, y + 31, 18, y + 31);
            Graphics.DrawLine(whitePen, 10, y + 32, 12, y + 32);
            Graphics.DrawLine(whitePen, 16, y + 32, 19, y + 32);
            Graphics.DrawLine(whitePen, 9, y + 33, 12, y + 33);
            Graphics.DrawLine(whitePen, 16, y + 33, 20, y + 33);
            Graphics.DrawLine(whitePen, 9, y + 34, 13, y + 34);
            Graphics.DrawLine(whitePen, 16, y + 34, 21, y + 34);
            Graphics.DrawLine(whitePen, 9, y + 35, 13, y + 35);
            Graphics.DrawLine(whitePen, 16, y + 35, 22, y + 35);
            Graphics.DrawLine(whitePen, 9, y + 36, 13, y + 36);
            Graphics.DrawLine(whitePen, 16, y + 36, 22, y + 36);
        }
    }
}
