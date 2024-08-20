namespace Benchmarks
{
    public static class Heapsort
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

        static void Inner(int[] x, int n)
        {
            int i, j, k, m;

            // pass1 -- put vector in heap form
            // that is to say, guarantee that x(i)>=x(2*i) and x(i)>=x(2*i+1).
            // after pass 1, the largest item will be at x(1).
            for (i = 2; i <= n; i++)
            {
                j = i;
                k = j / 2;
                m = x[i];

                // 0 < k <= (n / 2)
                // 1 <= j <= n
                while (k > 0)
                {
                    if (m <= x[k])
                    {
                        break;
                    }
                    x[j] = x[k];
                    j = k;
                    k = k / 2;
                }
                x[j] = m;
            }

            // pass 2 --  swap first and last items.  now with the last
            // item correctly placed, consider the list shorter by one item.
            // restore the shortened list to heap sort, and repeat
            // process until list is only two items long.
            i = n;
            do
            {
                // do i = n to 2 by -1;
                m = x[i];
                x[i] = x[1];  // last item, i.e. item(i) now correct.
                j = 1;        // we now find the appropriate resting point for m
                k = 2;

                // 2 <= k < i ==> 2 <= k < n
                // 1 <= j < n
                while (k < i)
                {
                    if ((k + 1) < i)
                    {
                        if (x[k + 1] > x[k])
                        {
                            k = k + 1;
                        }
                    }
                    if (x[k] <= m)
                    {
                        break;
                    }

                    x[j] = x[k];
                    j = k;
                    k = k + k;
                }

                x[j] = m;
                i = i - 1;
            } while (i >= 2);
        }

        const int MaxNum = 500;
        const int Modulus = 0x20000;

        static bool Bench()
        {
            int[] buffer = new int[MaxNum + 1];

            for (int j = 0; j <= MaxNum; ++j)
            {
                int temp = Random(Modulus);
                if (temp < 0)
                {
                    temp = (-temp);
                }
                buffer[j] = temp;
            }

            Inner(buffer, MaxNum - 1);

            for (int j = 1; j < MaxNum - 1; ++j)
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