using System.Drawing;

namespace Snake
{
    public unsafe struct Snake
    {
        private int* _snakeXs;
        private int* _snakeYs;

        int _snakeHead;
        int _snakeTail;

        int _maxLength;

        public int ActualLength;
        public int DesiredLength;

        private Direction _direction;
        private Direction _oldDirection;

        public Direction Course
        {
            set
            {
                if (_oldDirection != _direction)
                    _oldDirection = _direction;

                // Ignore direction change of snake back on itself
                if (_direction - value != 2 && value - _direction != 2)
                    _direction = value;
            }
        }

        public unsafe Snake(int x, int y, int* snakeXs, int* snakeYs, int length, Direction direction)
        {
            _direction = direction;
            _oldDirection = direction;

            _snakeHead = 0;
            _snakeTail = 0;
            ActualLength = 1;
            DesiredLength = 1;

            _snakeXs = snakeXs;
            _snakeYs = snakeYs;

            _maxLength = length;

            _snakeXs[0] = x;
            _snakeYs[0] = y;
        }

        public unsafe bool Update()
        {
            // Workout where new head of snake will go
            var newHeadX = HeadX;
            var newHeadY = HeadY;
            switch (_direction)
            {
                case Direction.Left: newHeadX--; break;
                case Direction.Right: newHeadX++; break;
                case Direction.Up: newHeadY--; break;
                case Direction.Down: newHeadY++; break;
            }

            newHeadX = GraphicHelper.WrapAround(newHeadX, Graphics.ScreenWidth / 2);
            newHeadY = GraphicHelper.WrapAround(newHeadY, Graphics.ScreenHeight);

            // Does new head hit body of snake?
            if (SnakeHit(newHeadX, newHeadY))
            {
                return false;
            }

            // Add new head
            _snakeHead++;
            _snakeHead %= _maxLength;

            _snakeXs[_snakeHead] = newHeadX;
            _snakeYs[_snakeHead] = newHeadY;

            // Move tail of snake
            if (ActualLength == DesiredLength)
            {
                _snakeTail++;
                _snakeTail %= _maxLength;
            }
            else
            {
                ActualLength++;
            }

            return true;
        }

        // TODO: This slows down as the length of the snake increases
        // need to look at optimisations in codegen to improve the
        // speed of the generated code
        public readonly bool SnakeHit(int newHeadX, int newHeadY)
        {
            for (var i = 0; i < _maxLength; i++)
            {
                if (i >= _snakeTail &&
                    i <= _snakeHead &&
                    _snakeXs[i] == newHeadX &&
                    _snakeYs[i] == newHeadY)
                {
                    return true;
                }
            }

            return false;
        }

        public bool Extend()
        {
            if (ActualLength < _maxLength - 1)
            {
                DesiredLength++;
                return true;
            }
            return false;
        }

        public unsafe readonly bool HitTest(int x, int y)
        {
            if (_snakeXs[_snakeHead] == x && _snakeYs[_snakeHead] == y)
            {
                return true;
            }

            return false;
        }

        public int HeadX => _snakeXs[_snakeHead];
        public int HeadY => _snakeYs[_snakeHead];

        public int TailX => _snakeXs[_snakeTail];
        public int TailY => _snakeYs[_snakeTail];
    }

    public enum Direction
    {
        Up,         // 0
        Right,      // 1
        Down,       // 2
        Left        // 3
    }
}
