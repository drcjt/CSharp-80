namespace Snake
{
    public unsafe struct Snake
    {
        private int* _snakeXs;
        private int* _snakeYs;

        int _snakeHead = 0;
        int _snakeTail = 0;

        int _maxLength;

        public unsafe Snake(int x, int y, int* snakeXs, int* snakeYs, int length)
        {
            _snakeXs = snakeXs;
            _snakeYs = snakeYs;

            _maxLength = length;

            _snakeXs[0] = x;
            _snakeYs[0] = y;
        }

        public int Length = 2;

        public unsafe void Next(int x, int y)
        {
            _snakeHead = (_snakeHead + 1) % _maxLength;

            int distance = 0;
            if (_snakeHead > _snakeTail)
            {
                distance = _snakeHead - _snakeTail;
            }
            else
            {
                distance = _snakeHead + (_maxLength - _snakeTail);
            }

            if (distance == Length)
            {
                _snakeTail = (_snakeTail + 1) % _maxLength;
            }

            _snakeXs[_snakeHead] = x;
            _snakeYs[_snakeHead] = y;
        }

        public bool Extend()
        {
            if (Length < _maxLength)
            {
                Length++;
                return true;
            }
            return false;
        }

        public unsafe readonly bool HitTest(int x, int y)
        {
            for (int i = 0; i < Length; i++)
            {
                if (_snakeXs[(_snakeHead + i) % _maxLength] == x &&
                    _snakeYs[(_snakeHead + i) % _maxLength] == y)
                {
                    return true;
                }
            }

            return false;
        }

        public int HeadX => _snakeXs[_snakeHead];
        public int HeadY => _snakeYs[_snakeHead];

        public int TailX => _snakeXs[_snakeTail];
        public int TailY => _snakeYs[_snakeTail];
    }
}
