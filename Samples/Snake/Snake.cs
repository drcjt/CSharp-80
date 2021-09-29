namespace Snake
{
    public struct Snake
    {
        public int headX;
        public int headY;

        public unsafe Snake(int x, int y)
        {
            headX = x;
            headY = y;
        }
    }
}
