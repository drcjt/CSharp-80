namespace Benchmarks
{
    public static class Quicksort
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

        static void Quick(int lo, int hi, int[] arr)
        {

            int i, j;
            int pivot, temp;

            if (lo < hi)
            {
                for (i = lo, j = hi, pivot = arr[hi]; i < j;)
                {
                    while (i < j && arr[i] <= pivot)
                    {
                        ++i;
                    }
                    while (j > i && arr[j] >= pivot)
                    {
                        --j;
                    }
                    if (i < j)
                    {
                        temp = arr[i];
                        arr[i] = arr[j];
                        arr[j] = temp;
                    }
                }

                // need to swap the pivot and a[i](or a[j] as i==j) so
                // that the pivot will be at its final place in the sorted array

                if (i != hi)
                {
                    temp = arr[i];
                    arr[i] = pivot;
                    arr[hi] = temp;
                }
                Quick(lo, i - 1, arr);
                Quick(i + 1, hi, arr);
            }
        }

        const int MaxNum = 500;
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

            Quick(0, MaxNum - 1, buffer);

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