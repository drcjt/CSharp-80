namespace Benchmarks
{
    public static class CSieve
    {
        public const int Iterations = 1;
        const int Size = 4095;

        static bool Bench()
        {
            bool[] flags = new bool[Size + 1];
            int count = 0;
            for (int iter = 1; iter <= Iterations; iter++)
            {
                count = 0;

                // Initially, assume all are prime
                for (int i = 0; i <= Size; i++)
                {
                    flags[i] = true;
                }

                // Refine
                for (int i = 2; i <= Size; i++)
                {
                    if (flags[i])
                    {
                        // Found a prime
                        for (int k = i + i; k <= Size; k += i)
                        {
                            // Cancel its multiples
                            flags[k] = false;
                        }
                        count++;
                    }
                }
            }

            return (count == 564);
        }

        public static int Main()
        {
            return Bench() ? 0 : 1;
        }
    }
}