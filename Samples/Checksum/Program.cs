using System;

namespace MiniBCL
{
    public static class Program
    {
        public static void Main()
        {
            // Calculate checksums of TRS-80 Model 1 ROMs
            // See https://www.trs-80.com/wordpress/zaps-patches-pokes-tips/rom-comparison-page-model-i/
            // for valid checksums
            CalculateChecksum("ROM A = ", SumMemory(0, 4096));
            CalculateChecksum("ROM B = ", SumMemory(4096, 8192));
            CalculateChecksum("ROM C = ", SumMemory(8192, 12288));
        }

        private unsafe static int SumMemory(int startAddress, int endAddress)
        {
            var sum = 0;
            for (int i = startAddress; i < endAddress; i++)
            {
                sum += *((byte*)i);
            }

            return sum;
        }

        private static void CalculateChecksum(string message, int sum)
        {
            var s = 16;
            var x = 2;
            while (s < sum)
            {
                x++;
                s *= 16;
            }

            Console.Write(message);

            for (var l = x; l > 0; l--)
            {
                var n = sum / s;
                var a = 0;

                if (n > 9) a = 1;
                if (l < 5)
                {
                    Console.Write((char)(48 + n + 7 * a));
                }
                sum -= n * s;
                s /= 16;
            }
            Console.WriteLine();
        }
    }
}
