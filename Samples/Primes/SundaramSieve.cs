using System;

namespace Primes
{
    internal static class SundaramSieve
    {
        private unsafe static void ResetPrimes(int* primes, int length)
        {
            for (int i = 0; i < length; i++)
            {
                primes[i] = 1;
            }
        }

        public static unsafe int GeneratePrimes(int* primes, int* temp, int bound)
        {
            ResetPrimes(temp, bound);

            var startTime = DateTime.Now;

            var k = (bound - 1) >> 1;

            var maxVal = 0;
            var denominator = 0;

            for (var i = 1; i < bound; i++)
            {
                denominator = (i << 1) + 1;
                maxVal = (bound - 1) / denominator;
                for (var j = 1; j <= maxVal; j++)
                {
                    if (i + j * denominator < bound)
                    {
                        temp[i + j * denominator] = 0;
                    }
                }
            }

            for (var i = 1; i <= k; i++)
            {
                if (temp[i] == 1)
                {
                    primes[(i << 1) + 1] = 1;
                }
            }

            primes[2] = 1;

            var endTime = DateTime.Now;
            var elapsedTime = endTime.TotalSeconds - startTime.TotalSeconds;

            return elapsedTime;
        }
    }
}
