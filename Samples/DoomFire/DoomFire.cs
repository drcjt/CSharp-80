using System;
using System.Drawing;

namespace DoomFire
{
    public static unsafe class Program
    {
        const int Width = 50;
        const int Height = 20;

        static byte* address = (byte*)0;
        private static unsafe int RandomInt() => (int)(*address++ & 3);

        public static unsafe void Main()
        {
            Console.Clear();

            var firePixels = stackalloc byte[Width * Height];
            
            for (int i = 0; i < Width * Height; i++)
                firePixels[i] = 0;

            for (int i = 0; i < Width; i++)
                firePixels[(Height - 1) * Width + i] = 36;

            for (int i = 0; i < 10; i++)
                RenderEffect(firePixels);
        }

        static unsafe void RenderEffect(byte* firePixels)
        {
#if TIME_CODE
            var startTime = DateTime.Now;
#endif

            byte* currentSrc = firePixels + Width + 1;
            byte* endSrc = firePixels + Width + Width;
            byte* endSrc2 = firePixels + (Width * Height);

            while (currentSrc < endSrc)
            {
                byte* srcOffset = currentSrc++;

                while (srcOffset < endSrc2)
                {
                    byte pixel = *srcOffset;
                    if (pixel == 0)
                    {
                        *(srcOffset - Width) = 0;
                    }
                    else
                    {
                        var rand = RandomInt(); // (int)(*address++ & 3);
                        var dst = (srcOffset - rand) + 1;
                        *(dst - Width) = (byte)(pixel - (rand & 1));
                    }
                    srcOffset += Width;
                }
            }

#if TIME_CODE
            var endTime = DateTime.Now;
            var elapsedTime = endTime.TotalSeconds - startTime.TotalSeconds;

            Console.SetCursorPosition(0, 12);
            Console.WriteLine(elapsedTime);

            startTime = DateTime.Now;
#endif

            int y = 0;
            byte* fp = firePixels;
            while (y < Height)
            {
                int x = 0;
                while (x < Width)
                {
                    var pixel = *fp++;

                    if (pixel > 18)
                    {
                        Graphics.SetPixel(x, y);
                    }
                    else
                    {
                        Graphics.SetPixel(x, y, Color.Black);
                    }
                    x++;
                }
                y++;
            }

#if TIME_CODE
            endTime = DateTime.Now;
            elapsedTime = endTime.TotalSeconds - startTime.TotalSeconds;

            Console.SetCursorPosition(10, 12);
            Console.WriteLine(elapsedTime);
#endif
        }
    }
}