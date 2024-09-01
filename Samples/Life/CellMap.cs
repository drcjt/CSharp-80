using Internal.Runtime.CompilerServices;
using System;

namespace Life
{
    public unsafe class CellMap
    {
        private readonly byte* _cells;
        private readonly byte* _workerCells;
        private readonly int _width;
        private readonly int _height;
        private readonly int _lengthInBytes;
        private readonly int _lastRowOffset;

        public CellMap(int h, int w, byte* cells, byte* workerCells)
        {
            _width = w;
            _height = h;
            _cells = cells;
            _workerCells = workerCells;
            _lengthInBytes = w * h;
            _lastRowOffset = _lengthInBytes - _width;

            Unsafe.InitBlock(_cells, 0, (uint)_lengthInBytes);
        }

        public void SetCell(int x, int y, byte* cellPtr)
        {
            *(cellPtr) |= 1;

            var xoleft = x == 0 ? _width - 1 : -1;
            var yoabove = y == 0 ? _lastRowOffset : -_width;
            var xoright = x == (_width - 1) ? -(_width - 1) : 1;
            var yobelow = y == (_height - 1) ? -_lastRowOffset : _width;

            *(cellPtr + yoabove + xoleft) += 2;
            *(cellPtr + yoabove) += 2;
            *(cellPtr + yoabove + xoright) += 2;
            *(cellPtr + xoleft) += 2;
            *(cellPtr + xoright) += 2;
            *(cellPtr + yobelow + xoleft) += 2;
            *(cellPtr + yobelow) += 2;
            *(cellPtr + yobelow + xoright) += 2;
        }

        public void ClearCell(int x, int y, byte* cellPtr)
        {
            *cellPtr &= 254; // clear bit 1

            var xoleft = x == 0 ? _width - 1 : -1;
            var yoabove = y == 0 ? _lastRowOffset : -_width;
            var xoright = x == (_width - 1) ? -(_width - 1) : 1;
            var yobelow = y == (_height - 1) ? -_lastRowOffset : _width;

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
                    byte* cellPtr = _cells + (y * _width) + x;
                    SetCell(x, y, cellPtr);
                    SetPixel(x, y);
                }
            }
        }

        public void NextGeneration()
        {
            // Create copy of cells to work through
            byte* currentGenerationCellPtr = _workerCells;
            byte* _nextGenerationCellPtr = _cells;

            Unsafe.CopyBlock(_workerCells, _cells, (uint)_lengthInBytes);

            for (var y = 0; y < _height; y++)
            {
                for (var x = 0; x < _width; x++)
                {
                    if (*currentGenerationCellPtr != 0)
                    {
                        var neighbourCount = *currentGenerationCellPtr >> 1;
                        if ((*currentGenerationCellPtr & 1) == 1)
                        {
                            // Cells which are on are have < 2 or > 3 neighbours are cleared
                            if ((neighbourCount != 2) && (neighbourCount != 3))
                            {
                                ClearCell(x, y, _nextGenerationCellPtr);
                                ClearPixel(x, y);
                            }
                        }
                        else
                        {
                            // Not set cells with three neighbours come back to life
                            if (neighbourCount == 3)
                            {
                                SetCell(x, y, _nextGenerationCellPtr);
                                SetPixel(x, y);
                            }
                        }
                    }

                    currentGenerationCellPtr++;
                    _nextGenerationCellPtr++;
                }
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
