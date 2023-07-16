using System;

namespace Primes
{
    public static class Program
    {
        public unsafe static int Main()
        {
            Console.Clear();

            const int Size = 200;
            var primes = stackalloc int[Size];
            var temp = stackalloc int[Size];

            RunAdvancedSieve(primes, Size);

            RunSieve(primes, Size);

            RunAtkinSieve(primes, Size);

            RunSundaramSieve(primes, Size, temp);

            RunTrialDivision(primes, Size);

            return 0;
        }

        private unsafe static void RunSieve(int* primes, int length)
        {
            Console.WriteLine("Original Sieve:");

            ResetPrimes(primes, length, 0);

            var elapsedTime = Sieve.GeneratePrimes(primes, length);
            PrimesWriter.Write(primes, length, elapsedTime);
        }

        private unsafe static void RunTrialDivision(int* primes, int length)
        {
            Console.WriteLine("Trial Division:");

            ResetPrimes(primes, length);
            var elapsedTime = TrialDivision.GeneratePrimes(primes, length);
            PrimesWriter.Write(primes, length, elapsedTime);
        }

        private unsafe static void RunAtkinSieve(int* primes, int length)
        {
            Console.WriteLine("Atkin Sieve:");

            ResetPrimes(primes, length, 0);
            var elapsedTime = AtkinSieve.GeneratePrimes(primes, length);
            PrimesWriter.Write(primes, length, elapsedTime);
        }

        private unsafe static void RunAdvancedSieve(int* primes, int length)
        {
            Console.WriteLine("Advanced Sieve:");

            ResetPrimes(primes, length);
            var elapsedTime2 = EratosthenesSieve.GeneratePrimes(primes, length);
            PrimesWriter.Write(primes, length, elapsedTime2);
        }

        private unsafe static void RunSundaramSieve(int* primes, int length, int* temp)
        {
            Console.WriteLine("Sundaram Sieve:");

            ResetPrimes(primes, length, 0);
            var elapsedTime = SundaramSieve.GeneratePrimes(primes, temp, length);
            PrimesWriter.Write(primes, length, elapsedTime);
        }

        private unsafe static void ResetPrimes(int* primes, int length, int value = 1)
        {
            for (int i = 2; i < length; i++) 
            { 
                primes[i] = value; 
            }

            primes[0] = 0;
            primes[1] = 0;
        }
    }
}