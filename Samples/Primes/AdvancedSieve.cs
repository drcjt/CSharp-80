using System;

namespace Primes
{
    internal static class EratosthenesSieve
    {
        public static unsafe int GeneratePrimes(int* isPrime, int bound)
        {
            var startTime = Environment.GetDateTime();

            var thisFactor = 2;

            while (thisFactor * thisFactor <= bound)
            {
                var mark = thisFactor + thisFactor;
                while (mark < bound)
                {
                    isPrime[mark] = 0;
                    mark += thisFactor;
                }

                thisFactor++;
                while (isPrime[thisFactor] == 0)
                {
                    thisFactor++;
                }
            }

            var endTime = Environment.GetDateTime();
            var elapsedTime = endTime.TotalSeconds - startTime.TotalSeconds;

            return elapsedTime;
        }
    }
}
