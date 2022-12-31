namespace System
{
    public sealed class Random
    {
        public uint _val;

        public Random() : this(Environment.TickCount)
        {
        }

        public Random(int Seed) 
        {
            _val = (uint)Seed;
        }

        public int Next() 
        {
            _val = (1103515245 * _val + 12345) % 2147483648;
            return (int)_val;
        }

        public int Next(int maxVlaue)
        {
            return Next() % maxVlaue;
        }
   }
}
