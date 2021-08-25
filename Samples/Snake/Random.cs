namespace Snake
{
    public struct Random
    {
        public uint _val;

        public Random(uint seed)
        {
            _val = seed;
        }

        // TODO: need to implement dup opcode properly so this can use expression body syntax
        public uint Next()
        {
            _val = (1103515245 * _val + 12345) % 2147483648;
            return _val;
        }
    }
}
