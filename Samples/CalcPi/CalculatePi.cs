using System;

namespace CalculatePi
{
    public static class CalculatePi
    {
        public static unsafe void Main()
        {
            Console.Clear();
            Console.WriteLine("Calculate Pi");
            Console.Write("How many digits of Pi do you wish to calculate? ");
            var digits = int.Parse(Console.ReadLine());
            DisplayDigitsOfPi(digits);
        }

        /// <summary>
        /// Calculate digits of pi using spigot algorithm.
        /// 
        /// See "A Spigot Algorithm for the Digits of Pi" by Stanley Rabinowitz and Stan Wagon
        /// http://www.stanleyrabinowitz.com/bibliography/spigot.pdf
        /// 
        /// </summary>
        /// <param name="digits">number of digits of pi to calculate</param>
        public static void DisplayDigitsOfPi(int digits)
        {
            digits++;

            int length = digits * 10 / 3 + 2;
            uint[] x = new uint[length];
            uint[] r = new uint[length];

            uint[] pi = new uint[digits];

            for (int j = 0; j < x.Length; j++)
                x[j] = 20;

            for (int i = 0; i < digits; i++)
            {
                Console.Write('.');
                uint carry = 0;
                for (int j = 0; j < length; j++)
                {
                    uint num = (uint)(length - j - 1);
                    uint dem = num * 2 + 1;

                    x[j] += carry;

                    uint q = x[j] / dem;
                    r[j] = x[j] % dem;

                    carry = q * num;
                }

                pi[i] = (x[length - 1] / 10);

                r[length - 1] = x[length - 1] % 10;

                for (int j = 0; j < length; j++)
                    x[j] = r[j] * 10;
            }

            uint c = 0;
            for (int i = digits - 1; i >= 0; i--)
            {
                pi[i] += c;               
                c = pi[i] / 10;
            }

            Console.WriteLine("PI = ");
            for (int i = 0; i < digits - 1; i++)
            {
                Console.Write(pi[i] % 10);
                if (i == 0)
                    Console.Write('.');
            }
        }
    }
}
