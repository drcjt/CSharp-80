namespace Benchmarks
{
    public static class Bubblesort
    {
        private static int _seed = 7;
        static int Random(int size)
        {
            unchecked
            {
                _seed = _seed * 25173 + 13849;
            }
            return (_seed % size);
        }

        static void SortArray(int[] tab, int last)
        {
            bool swap;
            int temp;
            do
            {
                swap = false;
                for (int i = 0; i < last; i++)
                {
                    if (tab[i] > tab[i + 1])
                    {
                        temp = tab[i];
                        tab[i] = tab[i + 1];
                        tab[i + 1] = temp;
                        swap = true;
                    }
                }
            }
            while (swap);
        }

        const int MaxNum = 100;
        const int Modulus = 0x20000;
        static bool Bench()
        {
            int[] buffer = new int[MaxNum];

            for (int j = 0; j < MaxNum; ++j)
            {
                int temp = Random(Modulus);
                if (temp < 0)
                {
                    temp = (-temp);
                }
                buffer[j] = temp;
            }

            SortArray(buffer, MaxNum - 1);

            for (int j = 0; j < MaxNum - 1; ++j)
            {
                if (buffer[j] > buffer[j + 1])
                {
                    return false;
                }
            }

            return true;
        }

        public static int Main()
        {
            return Bench() ? 0 : 1;
        }
    }
}