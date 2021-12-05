namespace Snake
{
    public static class GraphicHelper
    {
        public static int WrapAround(int coordinate, int max)
        {
            coordinate %= max + 1;
            return (coordinate < 0) ? max : coordinate;
        }
    }
}
