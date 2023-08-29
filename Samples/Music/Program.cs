using System;
using System.Threading;

namespace MiniBCL
{
    public static class Program
    {
        public static void Main()
        {
            Console.Clear();

            Console.WriteLine("Music Samples");
            Console.WriteLine("First a musical scale");
            Scale();

            Thread.Sleep(2000);

            Console.WriteLine("Now for all you star wars fans");
            ImperialMarch();

            Thread.Sleep(2000);

            Console.WriteLine("And should you choose to accept it");
            MissionImpossible();
        }

        private static void MissionImpossible()
        {
            // Mission impossible
            Console.Beep(784, 150);
            Thread.Sleep(300);
            Console.Beep(784, 150);
            Thread.Sleep(300);
            Console.Beep(932, 150);
            Thread.Sleep(150);
            Console.Beep(1047, 150);
            Thread.Sleep(150);
            Console.Beep(784, 150);
            Thread.Sleep(300);
            Console.Beep(784, 150);
            Thread.Sleep(300);
            Console.Beep(699, 150);
            Thread.Sleep(150);
            Console.Beep(740, 150);
            Thread.Sleep(150);
            Console.Beep(784, 150);
            Thread.Sleep(300);
            Console.Beep(784, 150);
            Thread.Sleep(300);
            Console.Beep(932, 150);
            Thread.Sleep(150);
            Console.Beep(1047, 150);
            Thread.Sleep(150);
            Console.Beep(784, 150);
            Thread.Sleep(300);
            Console.Beep(784, 150);
            Thread.Sleep(300);
            Console.Beep(699, 150);
            Thread.Sleep(150);
            Console.Beep(740, 150);
            Thread.Sleep(150);
            Console.Beep(932, 150);
            Console.Beep(784, 150);
            Console.Beep(587, 1200);
            Thread.Sleep(75);
            Console.Beep(932, 150);
            Console.Beep(784, 150);
            Console.Beep(554, 1200);
            Thread.Sleep(75);
            Console.Beep(932, 150);
            Console.Beep(784, 150);
            Console.Beep(523, 1200);
            Thread.Sleep(150);
            Console.Beep(466, 150);
            Console.Beep(523, 150);
        }

        private static void ImperialMarch()
        {
            // Imperial March
            Console.Beep(440, 500);
            Console.Beep(440, 500);
            Console.Beep(440, 500);
            Console.Beep(349, 350);
            Console.Beep(523, 150);
            Console.Beep(440, 500);
            Console.Beep(349, 350);
            Console.Beep(523, 150);
            Console.Beep(440, 1000);
            Console.Beep(659, 500);
            Console.Beep(659, 500);
            Console.Beep(659, 500);
            Console.Beep(698, 350);
            Console.Beep(523, 150);
            Console.Beep(415, 500);
            Console.Beep(349, 350);
            Console.Beep(523, 150);
            Console.Beep(440, 1000);
        }

        private static void Scale()
        {
            // Musical scale
            Console.Beep(440, 400);     // A
            Console.Beep(493, 400);     // B
            Console.Beep(523, 400);     // C
            Console.Beep(587, 400);     // D
            Console.Beep(659, 400);     // E
            Console.Beep(698, 400);     // F
            Console.Beep(783, 400);     // G
            Console.Beep(523, 400);     // C
        }
    }
}
