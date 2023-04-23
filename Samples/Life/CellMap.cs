using Internal.Runtime.CompilerServices;
using System;

namespace Life
{
    public unsafe class CellMap
    {
        private readonly byte* _cells;
        private readonly int _width;
        private readonly int _height;
        private readonly int _lengthInBytes;

        public CellMap(int h, int w, byte* cells)
        {
            _width = w;
            _height = h;
            _cells = cells;
            _lengthInBytes = w * h;

            Unsafe.InitBlock(_cells, 0, (uint)_lengthInBytes);
        }

        public void SetCell(int x, int y)
        {
            byte* cellPtr = _cells + (y * _width) + x;
            *(cellPtr) |= 1;

            var xoleft = x == 0 ? _width - 1 : -1;
            var yoabove = y == 0 ? _lengthInBytes - _width : -_width;
            var xoright = x == (_width - 1) ? -(_width - 1) : 1;
            var yobelow = y == (_height - 1) ? -(_lengthInBytes - _width) : _width;

            *(cellPtr + yoabove + xoleft) += 2;
            *(cellPtr + yoabove) += 2;
            *(cellPtr + yoabove + xoright) += 2;
            *(cellPtr + xoleft) += 2;
            *(cellPtr + xoright) += 2;
            *(cellPtr + yobelow + xoleft) += 2;
            *(cellPtr + yobelow) += 2;
            *(cellPtr + yobelow + xoright) += 2;
        }

        public void ClearCell(int x, int y)
        {
            byte* cellPtr = _cells + (y * _width) + x;
            *cellPtr &= 254; // clear bit 1

            var xoleft = x == 0 ? _width - 1 : -1;
            var yoabove = y == 0 ? _lengthInBytes - _width : -_width;
            var xoright = x == (_width - 1) ? -(_width - 1) : 1;
            var yobelow = y == (_height - 1) ? -(_lengthInBytes - _width) : _width;

            *(cellPtr + yoabove + xoleft) -= 2;
            *(cellPtr + yoabove) -= 2;
            *(cellPtr + yoabove + xoright) -= 2;
            *(cellPtr + xoleft) -= 2;
            *(cellPtr + xoright) -= 2;
            *(cellPtr + yobelow + xoleft) -= 2;
            *(cellPtr + yobelow) -= 2;
            *(cellPtr + yobelow + xoright) -= 2;
        }

        public int CellState(int x, int y)
        {
            return *(_cells + (y * _width) + x) & 1;
        }

        public void Init()
        {
            var random = new LehmerRandom(123);

            for (var initLength = (_height * _width) / 2; initLength > 0; initLength--)
            {
                var x = random.Next(_width);
                var y = random.Next(_height);
                if (CellState(x, y) == 0)
                {
                    SetCell(x, y);
                    SetPixel(x, y);
                }
            }
        }

        public void NextGeneration()
        {
            // Create copy of cells to work through
            byte* cellPtr = stackalloc byte[_lengthInBytes];
            Unsafe.CopyBlock(cellPtr, _cells, (uint)_lengthInBytes);

            for (var y = 0; y < _height; y++)
            {
                int x = 0;
                do
                {
                    // Skip cells which are not set and have no neighbours
                    while (*cellPtr == 0 && x < _width)
                    {
                        cellPtr++;
                        x++;
                    }

                    if (x < _width)
                    {
                        var neighbourCount = *cellPtr >> 1;
                        if ((*cellPtr & 1) == 1)
                        {
                            // Cells which are on are have < 2 or > 3 neighbours are cleared
                            if ((neighbourCount != 2) && (neighbourCount != 3))
                            {
                                ClearCell(x, y);
                                ClearPixel(x, y);
                            }
                        }
                        else
                        {
                            // Not set cells with three neighbours come back to life
                            if (neighbourCount == 3)
                            {
                                SetCell(x, y);
                                SetPixel(x, y);
                            }
                        }

                        cellPtr++;
                        x++;
                    }
                } while (x < _width);
            }
        }

        public static void SetPixel(int x, int y)
        {
            Console.SetCursorPosition((sbyte)x, (sbyte)(y + 1));
            Console.Write("*");
        }

        public static void ClearPixel(int x, int y)
        {
            Console.SetCursorPosition((sbyte)x, (sbyte)(y + 1));
            Console.Write(" ");
        }
    }
}
