using Primes;
using System;

namespace MiniBCL
{
    public static class Program
    {
        public unsafe static void Main()
        {
            Console.Clear();

            const int Size = 1000;
            var primes = stackalloc int[Size];
            var temp = stackalloc int[Size];

            RunSieve(primes, Size);
            RunSimpleSieve(primes, Size);
            RunAdvancedSieve(primes, Size);
            RunSundaramSieve(primes, Size, temp);
        }

        private unsafe static void RunSieve(int* primes, int length)
        {
            Console.WriteLine("Original Sieve:");

            ResetPrimes(primes, length);
            var elapsedTime = Sieve.GeneratePrimes(primes, length);
            PrimesWriter.Write(primes, length, elapsedTime);
        }

        private unsafe static void RunSimpleSieve(int* primes, int length)
        {
            Console.WriteLine("Trial Division:");

            ResetPrimes(primes, length);
            var elapsedTime = TrialDivision.GeneratePrimes(primes, length);
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
            for (int i = 0; i < length; i++) 
            { 
                primes[i] = value; 
            }
        }
    }
}
