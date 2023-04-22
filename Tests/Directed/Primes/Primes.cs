namespace Primes
{
    public static class Tests
    {
        public static int CountPrimesUpto(int bound)
        {
            var isPrime = new int[bound];
            for (int i = 2; i < bound; i++)
            {
                isPrime[i] = 1;
            }

            isPrime[0] = 0;
            isPrime[1] = 0;

            var thisFactor = 2;
            while (thisFactor * thisFactor <= bound)
            {
                var mark = thisFactor + thisFactor;
                while (mark <= bound)
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

            var totalPrimes = 0;
            for (int i = 0; i < bound; i++)
            {
                totalPrimes += isPrime[i];
            }

            return totalPrimes;
        }

        public static int Main()
        {
            if (CountPrimesUpto(1000) != 168)
            {
                return 1;
            }

            return 0;
        }
    }
}