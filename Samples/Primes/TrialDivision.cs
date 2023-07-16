using System;

namespace Primes
{
    internal static class TrialDivision
    {
        public unsafe static int GeneratePrimes(int* primes, int limit)
        {
            var startTime = Environment.GetDateTime();

            for (var i = 2; i < limit; i++)
            {
                var isPrime = true;
                var n = Sqrt(i);

                for (var j = 2; j <= n; j++)
                {
                    if (i % j == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }

                if (!isPrime)
                {
                    primes[i] = 0;
                }
            }

            var endTime = Environment.GetDateTime();
            var elapsedTime = endTime.TotalSeconds - startTime.TotalSeconds;

            return elapsedTime;
        }

        private static int Sqrt(int num)
        {
            int ret = 0;

            for (var i = 15; i >= 0; i--)
            {
                int temp = ret | (1 << i);
                if (temp * temp <= num)
                {
                    ret = temp;
                }
            }

            return ret;
        }
    }
}
