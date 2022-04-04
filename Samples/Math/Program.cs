using System;

namespace MiniBCL
{
    public static class Program
    {
        public static unsafe void Main()
        {
            Console.Clear();

            const int Size = 8192;

            var isPrimes = stackalloc int[Size];
            for (int i = 0; i < Size; i++) { isPrimes[i] = 0; }

            var elapsedTime = RunSieve(isPrimes, Size);

            for (int i = 0; i < Size; i++)
            {
                if (isPrimes[i] == 1)
                {
                    Console.Write(i);
                    Console.Write(",");
                }
            }

            Console.WriteLine();
            Console.Write("Elapsed Time: ");
            Console.Write(elapsedTime);
            Console.WriteLine();
        }

        private static int DateTimeToSeconds(DateTime datetime)
        {
            return datetime.Second + (datetime.Minute * 60) + (datetime.Hour * 60 * 60);
        }

        private static unsafe int RunSieve(int* isPrime, int bound)
        {
            var startTime = Environment.GetDateTime();

            //bool[] isPrime = new bool[bound];
            isPrime[2] = 1;
            isPrime[3] = 1;

            // Taking into account that all primes greater than 2 and 3
            // Are of the form 6k+1 or 6k-1
            for (int k = 6; k < bound; k += 6)
            {
                if (k + 1 < bound) isPrime[k + 1] = 1;
                isPrime[k - 1] = 1;
            }

            // At this point we still have some numbers that aren't prime marked as prime
            // So we go over them with a sieve, also we can start at 3 for obvious reasons
            for (int i = 3; i * i <= bound; i += 2)
            {
                if (isPrime[i] == 1)
                {
                    // Can this be optimized?
                    for (int j = i; j * i <= bound; j++)
                        isPrime[i * j] = 0;
                }
            }

            var endTime = Environment.GetDateTime();
            var elapsedTime = endTime.TotalSeconds - startTime.TotalSeconds;

            return elapsedTime;
        }
    }
}
