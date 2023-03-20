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
                if (Console.KeyAvailable)
                {
                    var cki = Console.ReadKey(true);

                    if (cki.KeyChar != 0)
                    {
                        int keyChar = cki.KeyChar;

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
                            color = color == Color.Black ? Color.White : Color.Black;
                        }
                        else if (keyChar == 81)
                        {
                            // Press Q to quit
                            break;
                        }

                        Graphics.SetPixel(currentX, currentY, cursorColor);
                    }
                }

                if (Environment.TickCount - ticks > 300)
                {
                    Graphics.SetPixel(currentX, currentY, cursorColor);
                    cursorColor = cursorColor == Color.Black ? Color.White : Color.Black;
                    ticks = Environment.TickCount;
                }
            }
        }
    }
}
