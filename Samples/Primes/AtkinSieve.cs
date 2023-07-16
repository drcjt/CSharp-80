using System;

namespace Primes
{
    internal static class AtkinSieve
    {
        private static int Xor(int a, int b)
        {
            if (a == b) return 0;
            return 1;
        }

        public unsafe static int GeneratePrimes(int* primes, int limit)
        {
            var startTime = Environment.GetDateTime();

            var sqrt = Sqrt(limit);

            for (var x = 1; x <= sqrt; x++)
            {
                var xx = x * x;
                for (var y = 1; y <= sqrt; y++)
                {
                    var yy = y * y;
                    var n = 4 * xx + yy;
                    if (n < limit && (n % 12 == 1 || n % 12 == 5))
                        primes[n] = Xor(primes[n], 1);

                    n = 3 * xx + yy;
                    if (n < limit && n % 12 == 7)
                        primes[n] = Xor(primes[n], 1);

                    n = 3 * xx - yy;
                    if (x > y && n < limit && n % 12 == 11)
                        primes[n] = Xor(primes[n], 1);
                }
            }

            for (var n = 5; n <= sqrt; n++)
            {
                if (primes[n] == 1)
                {
                    var nn = n * n;
                    for (var k = nn; k < limit; k += nn)
                    {
                        primes[k] = 0;
                    }
                }
            }

            primes[2] = 1;
            primes[3] = 1;

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
