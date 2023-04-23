namespace Life
{
    internal class LehmerRandom
    {
        private const int a = 16807;
        private const int m = 2147483647;
        private const int q = 127773;
        private const int r = 2836;
        private int seed;

        public LehmerRandom(int seed)
        {
            this.seed = seed;
        }
        public int Next()
        {
            int hi = seed / q;
            int lo = seed % q;
            seed = (a * lo) - (r * hi);
            if (seed <= 0)
            {
                seed = seed + m;
            }
            return seed;
        }

        public int Next(int value)
        {
            return Next() % value;
        }
    }
}
