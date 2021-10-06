using System;
using System.Drawing;

namespace Paint
{
    public static class Program
    {
        public static void Main()
        {
            Console.Clear();

            int currentX = Graphics.ScreenWidth / 2;
            int currentY = Graphics.ScreenHeight / 2;

            Graphics.SetPixel(currentX, currentY, Color.White);

            int ticks = Environment.TickCount;

            Color color = Color.White;
            Color cursorColor = Color.White;
            while (true)
            {
                int keyChar = Console.KbdScan();

                if (keyChar != 0)
                {
                    Graphics.SetPixel(currentX, currentY, color);

                    if (keyChar == 10)
                    {
                        currentY++;
                    }
                    else if (keyChar == 91)
                    {
                        currentY--;
                    }
                    else if (keyChar == 8)
                    {
                        currentX--;
                    }
                    else if (keyChar == 9)
                    {
                        currentX++;
                    }
                    else if (keyChar == 32)
                    {
                        color = (int)color == (int)Color.Black ? Color.White : Color.Black;
                    }
                }
                else
                {
                    if (Environment.TickCount - ticks > 500)
                    {
                        Graphics.SetPixel(currentX, currentY, cursorColor);
                        cursorColor = (int)cursorColor == (int)Color.Black ? Color.White : Color.Black;
                        ticks = Environment.TickCount;
                    }
                }
            }
        }
    }
}
