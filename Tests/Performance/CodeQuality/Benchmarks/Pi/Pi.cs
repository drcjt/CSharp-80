namespace Benchmarks
{
    public static class Pi
    {
        static int[] ComputePi(int[] a)
        {
            //const int d = 4;
            const int r = 10000;
            const int n = 25;
            const int m = 332; // (int)(3.322 * n * d);
            int[] digits = new int[n];
            int i, k, q;

            for (i = 0; i <= m; i++)
            {
                a[i] = 2;
            }

            a[m] = 4;

            for (i = 1; i <= n; i++)
            {
                q = 0;
                for (k = m; k > 0; k--)
                {
                    a[k] = a[k] * r + q;
                    q = a[k] / (2 * k + 1);
                    a[k] -= (2 * k + 1) * q;
                    q *= k;
                }
                a[0] = a[0] * r + q;
                q = a[0] / r;
                a[0] -= q * r;
                digits[i - 1] = q;
            }

            return digits;
        }

        static bool Bench()
        {
            int[] a = new int[333];
            int[] digits = ComputePi(a);
            return (digits[0] == 3 && digits[1] == 1415 && digits[2] == 9265 && digits[24] == 2117);
        }

        public static int Main()
        {
            return Bench() ? 0 : 1;
        }
    }
}