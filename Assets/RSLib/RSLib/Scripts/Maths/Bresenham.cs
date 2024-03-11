namespace RSLib.Maths
{
    public static class Bresenham
    {
        public static System.Collections.Generic.List<UnityEngine.Vector2Int> ComputeBresenhamLine(UnityEngine.Vector2Int a, UnityEngine.Vector2Int b, bool addLastPoint = true)
        {
            System.Collections.Generic.List<UnityEngine.Vector2Int> line = new System.Collections.Generic.List<UnityEngine.Vector2Int>();
            ComputeBresenhamLine(line, a, b, addLastPoint);
            return line;
        }

        public static System.Collections.Generic.List<UnityEngine.Vector2Int> ComputeBresenhamLine(int ax, int ay, int bx, int by, bool addLastPoint = true)
        {
            System.Collections.Generic.List<UnityEngine.Vector2Int> line = new System.Collections.Generic.List<UnityEngine.Vector2Int>();
            ComputeBresenhamLine(line, ax, ay, bx, by, addLastPoint);
            return line;
        }

        public static void ComputeBresenhamLine(System.Collections.Generic.List<UnityEngine.Vector2Int> line, UnityEngine.Vector2Int a, UnityEngine.Vector2Int b, bool addLastPoint = true)
        {
            ComputeBresenhamLine(line, a.x, a.y, b.x, b.y, addLastPoint);
        }

        public static void ComputeBresenhamLine(System.Collections.Generic.List<UnityEngine.Vector2Int> line, int ax, int ay, int bx, int by, bool addLastPoint = true)
        {
            int dx;
            int dy;

            if ((dx = bx - ax) != 0)
            {
                if (dx > 0)
                {
                    if ((dy = by - ay) != 0)
                    {
                        if (dy > 0)
                        {
                            if (dx >= dy)
                            {
                                int e = dx;
                                dx = e * 2;
                                dy *= 2;

                                while (true)
                                {
                                    line.Add(new UnityEngine.Vector2Int(ax, ay));
                                    if (++ax == bx)
                                        break;

                                    if ((e -= dy) < 0)
                                    {
                                        ay++;
                                        e += dx;
                                    }
                                }
                            }
                            else
                            {
                                int e = dy;
                                dy = e * 2;
                                dx *= 2;

                                while (true)
                                {
                                    line.Add(new UnityEngine.Vector2Int(ax, ay));
                                    if (++ay == by)
                                        break;

                                    if ((e -= dx) < 0)
                                    {
                                        ax++;
                                        e += dy;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (dx >= -dy)
                            {
                                int e = dx;
                                dx = e * 2;
                                dy *= 2;

                                while (true)
                                {
                                    line.Add(new UnityEngine.Vector2Int(ax, ay));
                                    if (++ax == bx)
                                        break;

                                    if ((e += dy) < 0)
                                    {
                                        ay--;
                                        e += dx;
                                    }
                                }
                            }
                            else
                            {
                                int e = dy;
                                dy = e * 2;
                                dx *= 2;

                                while (true)
                                {
                                    line.Add(new UnityEngine.Vector2Int(ax, ay));
                                    if (--ay == by)
                                        break;

                                    if ((e += dx) > 0)
                                    {
                                        ax++;
                                        e += dy;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        do line.Add(new UnityEngine.Vector2Int(ax, ay));
                        while (++ax != bx);
                    }
                }
                else
                {
                    if ((dy = by - ay) != 0)
                    {
                        if (dy > 0)
                        {
                            if (-dx >= dy)
                            {
                                int e = dx;
                                dx = e * 2;
                                dy *= 2;

                                while (true)
                                {
                                    line.Add(new UnityEngine.Vector2Int(ax, ay));
                                    if (--ax == bx)
                                        break;

                                    if ((e += dy) >= 0)
                                    {
                                        ay++;
                                        e += dx;
                                    }
                                }
                            }
                            else
                            {
                                int e = dy;
                                dy = e * 2;
                                dx *= 2;

                                while (true)
                                {
                                    line.Add(new UnityEngine.Vector2Int(ax, ay));
                                    if (++ay == by)
                                        break;

                                    if ((e += dx) <= 0)
                                    {
                                        ax--;
                                        e += dy;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (dx <= dy)
                            {
                                int e = dx;
                                dx = e * 2;
                                dy *= 2;

                                while (true)
                                {
                                    line.Add(new UnityEngine.Vector2Int(ax, ay));
                                    if (--ax == bx)
                                        break;

                                    if ((e -= dy) >= 0)
                                    {
                                        ay--;
                                        e += dx;
                                    }
                                }
                            }
                            else
                            {
                                int e = dy;
                                dy = e * 2;
                                dx *= 2;

                                while (true)
                                {
                                    line.Add(new UnityEngine.Vector2Int(ax, ay));
                                    if (--ay == by)
                                        break;

                                    if ((e -= dx) >= 0)
                                    {
                                        ax--;
                                        e += dy;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        do line.Add(new UnityEngine.Vector2Int(ax, ay));
                        while (--ax != bx);
                    }
                }
            }
            else
            {
                if ((dy = by - ay) != 0)
                {
                    if (dy > 0)
                    {
                        do line.Add(new UnityEngine.Vector2Int(ax, ay));
                        while (++ay != by);
                    }
                    else
                    {
                        do line.Add(new UnityEngine.Vector2Int(ax, ay));
                        while (--ay != by);
                    }
                }
            }

            if (addLastPoint)
                line.Add(new UnityEngine.Vector2Int(bx, by));
        }

        public static System.Collections.Generic.List<UnityEngine.Vector2Int> ComputeBresenhamLineWithoutDiagonals(UnityEngine.Vector2Int a, UnityEngine.Vector2Int b)
        {
            System.Collections.Generic.List<UnityEngine.Vector2Int> line = new System.Collections.Generic.List<UnityEngine.Vector2Int>();
            ComputeBresenhamLineWithoutDiagonals(line, a, b);
            return line;
        }

        public static System.Collections.Generic.List<UnityEngine.Vector2Int> ComputeBresenhamLineWithoutDiagonals(int ax, int ay, int bx, int by)
        {
            System.Collections.Generic.List<UnityEngine.Vector2Int> line = new System.Collections.Generic.List<UnityEngine.Vector2Int>();
            ComputeBresenhamLineWithoutDiagonals(line, ax, ay, bx, by);
            return line;
        }

        public static void ComputeBresenhamLineWithoutDiagonals(System.Collections.Generic.List<UnityEngine.Vector2Int> line, UnityEngine.Vector2Int a, UnityEngine.Vector2Int b)
        {
            ComputeBresenhamLineWithoutDiagonals(line, a.x, a.y, b.x, b.y);
        }

        public static void ComputeBresenhamLineWithoutDiagonals(System.Collections.Generic.List<UnityEngine.Vector2Int> line, int ax, int ay, int bx, int by)
        {
            int dx = UnityEngine.Mathf.Abs(bx - ax);
            int dy = -UnityEngine.Mathf.Abs(by - ay);
            int xStep = ax < bx ? 1 : -1;
            int yStep = ay < by ? 1 : -1;
            int e = dx + dy;

            line.Add(new UnityEngine.Vector2Int(ax, ay));

            while (ax != bx || ay != by)
            {
                if (2 * e - dy >= dx - 2 * e)
                {
                    e += dy;
                    ax += xStep;
                }
                else
                {
                    e += dx;
                    ay += yStep;
                }

                line.Add(new UnityEngine.Vector2Int(ax, ay));
            }
        }
    }
}