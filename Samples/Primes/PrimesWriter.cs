using System;

namespace Primes
{
    internal static class PrimesWriter
    {
        public unsafe static void Write(int* primes, int length, int elapsedTime)
        {
            var firstPrime = true;

            var totalPrimes = 0;
            for (int i = 0; i < length; i++)
            {
                if (primes[i] == 1)
                {
                    totalPrimes++;
                    if (!firstPrime)
                    {
                        Console.Write(',');
                    }
                    Console.Write(i);
                    firstPrime = false;
                }
            }

            Console.WriteLine();

            Console.WriteLine($"Total: {totalPrimes}");
            Console.WriteLine($"Elapsed Time (seconds): {elapsedTime}");
        }
    }
}
