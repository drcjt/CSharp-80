namespace System.Drawing
{
    public partial class Graphics
    {
        /// <summary>
        /// Draws a line connecting the two points specified by the coordinate points
        /// </summary>
        /// <param name="pen">Pen that determines the color, width, and style of the line.</param>
        /// <param name="x1">The x-coordinate of the first point</param>
        /// <param name="y1">The y-coordinate of the first point</param>
        /// <param name="x2">The x-coordinate of the second point</param>
        /// <param name="y2">The y-coordinate of the second point</param>
        public static void DrawLine(Pen pen, int x1, int y1, int x2, int y2)
        {
            // Bresenhams algorithm
            bool steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);
            if (steep)
            {
                int t = x1; // swap x0 and y0
                x1 = y1;
                y1 = t;
                t = x2; // swap x1 and y1
                x2 = y2;
                y2 = t;
            }
            if (x1 > x2)
            {
                int t = x1; // swap x0 and x1
                x1 = x2;
                x2 = t;
                t = y1; // swap y0 and y1
                y1 = y2;
                y2 = t;
            }

            int dx = x2 - x1;
            int dy = Math.Abs(y2 - y1);
            int error = dx / 2;
            int ystep = -1;
            if (y1 < y2) ystep = 1;

            int y = y1;
            for (int x = x1; x <= x2; x++)
            {
                SetPixel(steep ? y : x, steep ? x : y, pen.Color);

                error -= dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }
        }

        /// <summary>
        /// Draws an ellipse within the specified bounding box
        /// </summary>
        /// <param name="pen">Pen that determines the color, width, and style of the ellipse.</param>
        /// <param name="x">The x-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse.</param>
        /// <param name="y">The y-coordinate of the upper-left corner of the bounding rectangle that defines the ellipse.</param>
        /// <param name="width">Width of the bounding rectangle that defines the ellipse.</param>
        /// <param name="height">Height of the bounding rectangle that defines the ellipse.</param>
        public static void DrawEllipse(Pen pen, int x, int y, int width, int height)
        {
            int xb, yb, xc, yc;

            // Calculate height
            yb = yc = (2 * y + height) / 2;
            int qb = height; // (y0 < y1) ? (y1 - y0) : (y0 - y1);
            int qy = qb;
            int dy = qb / 2;
            if (qb % 2 != 0)
            {
                // Bounding box has even pixel height
                yc++;
            }

            // Calculate width
            xb = xc = (2 * x + width) / 2;
            int qa = width; // (x0 < x1) ? (x1 - x0) : (x0 - x1);

            int qasqr = qa * qa;
            int qbsqr = qb * qb;

            int qa2 = 2 * qasqr;
            int qa3 = 3 * qasqr;
            int qa4 = 4 * qasqr;
            int qa8 = 8 * qasqr;

            int qb2 = 2 * qbsqr;
            int qb3 = 3 * qbsqr;
            int qb8 = 8 * qbsqr;
            int qb4 = 4 * qbsqr;

            int qx = qa % 2;
            int dx = 0;
            int qt = qasqr + qbsqr - 2 * qasqr * qb;
            if (qx != 0)
            {
                // Bounding box has even pixel width
                xc++;
                qt += qb3;
            }

            // Start at (dx, dy) = (0, b) and iterate until (a, 0) is reached
            while (qy >= 0 && qx <= qa)
            {
                // Draw the new points
                DrawPoint(pen, xb - dx, yb - dy);
                if (dx != 0 || xb != xc)
                {
                    DrawPoint(pen,xc + dx, yb - dy);
                    if (dy != 0 || yb != yc)
                        DrawPoint(pen, xc + dx, yc + dy);
                }
                if (dy != 0 || yb != yc)
                {
                    DrawPoint(pen, xb - dx, yc + dy);
                }

                // If a (+1, 0) step stays inside the ellipse, do it
                if (qt + qb2 * qx + qb3 <= 0 || qt + qa2 * qy - qasqr <= 0)
                {
                    qt += qb8 + qb4 * qx;
                    dx++;
                    qx += 2;
                    // If a (0, -1) step stays outside the ellipse, do it
                }
                else if (qt - qa2 * qy + qa3 > 0)
                {
                    qt += qa8 - qa4 * qy;
                    dy--;
                    qy -= 2;
                    // Else step (+1, -1)
                }
                else
                {
                    qt += qb8 + qb4 * qx + qa8 - qa4 * qy;
                    dx++;
                    qx += 2;
                    dy--;
                    qy -= 2;
                }
            }   // End of while loop
            return;
        }

        private static void DrawPoint(Pen pen, int x, int y)
        {
            SetPixel(x, y, pen.Color);
        }
    }
}
