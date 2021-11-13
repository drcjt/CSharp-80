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
                SetPixel(steep ? y : x, steep ? x : y);

                error -= dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }
        }

        // TODO: This isn't working properly yet
        public static void DrawEllipse(int x0, int y0, int x1, int y1)
        {
            int xb, yb, xc, yc;

            yb = yc = (y0 + y1) / 2;
            int qb = (y0 < y1) ? (y1 - y0) : (y0 - y1);
            int qy = qb;
            int dy = qb / 2;
            if (qb % 2 != 0)
            {
                yc++;
            }

            xb = xc = (x0 + x1) / 2;
            int qa = (x0 < x1) ? (x1 - x0) : (x0 - x1);
            int qx = qa % 2;
            int dx = 0;
            int qt = (qa * qa) + (qb * qb) - (2 * qa * qa * qb);
            if (qx != 0)
            {
                xc++;
                qt += 3 * qb * qb;
            }

            while (qy >= 0 && qx <= qa)
            {
                DrawPoint(xb - dx, yb - dy);
                if (dx != 0 || xb != xc)
                {
                    DrawPoint(xc + dx, yb - dy);
                    if (dy != 0 || yb != yc)
                    {
                        DrawPoint(xc + dx, yc + dy);
                    }
                }
                if (dy != 0 || yb != yc)
                {
                    DrawPoint(xb - dx, yc + dy);
                }

                if (qt + (2 * qb * qb * qx) + (3 * qb * qb) <= 0 ||
                    qt + (2 * qa * qa * qy) - (qa * qa) <= 0)
                {
                    qt += (8 * qb * qb) + (4 * qb * qb * qx);
                    dx++;
                    qx += 2;
                }
                else if (qt - (2 * qa * qa * qy) + (3 * qa * qa) > 0)
                {
                    qt += (8 * qa * qa) - (4 * qa * qa * qy);
                    dy--;
                    qy -= 2;
                }
                else
                {
                    qt += (8 * qb * qb) + (4 * qb * qb * qx) + (8 * qa * qa) - (4 * qa * qa * qy);
                    dx++;
                    qx += 2;
                    dy--;
                    qy -= 2;
                }
            }
        }

        private static void DrawPoint(int x, int y)
        {
            SetPixel(x, y);
            //Console.Write(x);
            //Console.Write(", ");
            //Console.WriteLine(y);
        }


        // TODO: This isn't working properly yet
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">The x-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse.</param>
        /// <param name="y">The y-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse.</param>
        /// <param name="width">Width of the bounding rectangle that defines the ellipse.</param>
        /// <param name="height">Height of the bounding rectangle that defines the ellipse.</param>
        public static void DrawEllipse2(int x, int y, int width, int height)
        {
            // TODO: Consider using Modified McIlroy algorithm as per https://stackoverflow.com/questions/2914807/plot-ellipse-from-rectangle

            int xradius = width / 2;
            int yradius = height / 2;

            int cx = x + xradius;
            int cy = y + yradius;

            var twoasquare = 2 * xradius * xradius;
            var twobsquare = 2 * yradius * yradius;
            var xp = xradius;
            var yp = 0;
            var xchange = yradius * yradius * (1 - 2 * xradius);
            var ychange = xradius * xradius;
            var ellipseerror = 0;
            var stoppingx = twobsquare * xradius;
            var stoppingy = 0;

            while (stoppingx >= stoppingy)
            {
                Plot4EllipsePoints(cx, cy, xp, yp);
                yp++;
                stoppingy += twoasquare;
                ellipseerror += ychange;
                ychange += twoasquare;
                if ((2 * ellipseerror + xchange) > 0)
                {
                    xp--;
                    stoppingx -= twobsquare;
                    ellipseerror -= xchange;
                    xchange -= twobsquare;
                }
            }

            xp = 0;
            yp = yradius;
            xchange = yradius * yradius;
            ychange = xradius * xradius * (1 - 2 * yradius);
            ellipseerror = 0;
            stoppingx = 0;
            stoppingy = twoasquare * yradius;
            while (stoppingx <= stoppingy)
            {
                Plot4EllipsePoints(cx, cy, xp, yp);
                xp++;
                stoppingx += twobsquare;
                ellipseerror += xchange;
                xchange += twobsquare;
                if ((2 * ellipseerror + ychange) > 0)
                {
                    yp--;
                    stoppingy -= twoasquare;
                    ellipseerror -= ychange;
                    ychange -= twoasquare;
                }
            }
        }

        private static void Plot4EllipsePoints(int cx, int cy, int x, int y)
        {
            SetPixel(cx + x, cy + y);
            SetPixel(cx - x, cy + y);
            SetPixel(cx - x, cy - y);
            SetPixel(cx + x, cy - y);

            /*
            Console.WriteLine($"{cx + x}, {cy + y}");
            Console.WriteLine($"{cx - x}, {cy + y}");
            Console.WriteLine($"{cx - x}, {cy - y}");
            Console.WriteLine($"{cx + x}, {cy - y}");
            */
        }
    }
}
