namespace System.Drawing
{
    public partial class Graphics
    {
        /// <summary>
        /// Draws a line connecting the two points specified by the coordinate points
        /// </summary>
        /// <param name="x0">The x-coordinate of the first point</param>
        /// <param name="y0">The y-coordinate of the first point</param>
        /// <param name="x1">The x-coordinate of the second point</param>
        /// <param name="y1">The y-coordinate of the second point</param>
        public static void DrawLine(int x0, int y0, int x1, int y1)
        {
            // Bresenhams algorithm
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                int t = x0; // swap x0 and y0
                x0 = y0;
                y0 = t;
                t = x1; // swap x1 and y1
                x1 = y1;
                y1 = t;
            }
            if (x0 > x1)
            {
                int t = x0; // swap x0 and x1
                x0 = x1;
                x1 = t;
                t = y0; // swap y0 and y1
                y0 = y1;
                y1 = t;
            }

            int dx = x1 - x0;
            int dy = Math.Abs(y1 - y0);
            int error = dx / 2;
            int ystep = -1;
            if (y0 < y1) ystep = 1;
            int y = y0;

            for (int x = x0; x <= x1; x++)
            {
                int px = x;
                if (steep) px = y;
                int py = y;
                if (steep) py = x;

                SetPixel(px, py);

                error -= dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }
        }
    }
}
