using System.Drawing;

namespace Snake
{
    public unsafe struct Snake
    {
        // For inspiration on data structures see here
        // https://stackoverflow.com/questions/13094373/snake-in-assembly-what-datastructure-to-use?msclkid=277ab387ad4a11ec91846c3086c26b05

        private readonly int* _snakeXs;
        private readonly int* _snakeYs;

        private int _snakeHead;
        private int _snakeTail;

        private readonly int _maxLength;

        private int _actualLength;
        private int _desiredLength;

        private Direction _direction;
        private Direction _oldDirection;

        public void SetCourse(Direction newDirection)
        {
            if (_oldDirection != _direction)
                _oldDirection = _direction;

            // Ignore direction change of snake back on itself
            if (_direction - newDirection != 2 && newDirection - _direction != 2)
                _direction = newDirection;
        }

        private readonly int* _board;

        public unsafe Snake(int x, int y, int* snakeXs, int* snakeYs, int* board, int maxLength, Direction direction)
        {
            _direction = direction;
            _oldDirection = direction;

            _snakeHead = 0;
            _snakeTail = 0;
            _actualLength = 1;
            _desiredLength = 1;

            _snakeXs = snakeXs;
            _snakeYs = snakeYs;

            _maxLength = maxLength;
            _snakeXs[0] = x;
            _snakeYs[0] = y;

            _board = board;
        }

        private readonly unsafe bool GetBoardItem(int x, int y)
        {
            int bitIndex = (y * 128) + x;
            int bitMask = 1 << bitIndex;

            return ((_board[bitIndex >> 5]) & bitMask) != 0;
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
            newHeadY = GraphicHelper.WrapAround(newHeadY, Graphics.ScreenHeight - 3);

            // Does new head hit body of snake?
            if (GetBoardItem(newHeadX, newHeadY))
            {
                return false;
            }

            // Add new head
            _snakeHead++;
            _snakeHead %= _maxLength;

            _snakeXs[_snakeHead] = newHeadX;
            _snakeYs[_snakeHead] = newHeadY;

            // Move tail of snake
            if (_actualLength == _desiredLength)
            {
                _snakeTail++;
                _snakeTail %= _maxLength;
            }
            else
            {
                _actualLength++;
            }

            return true;
        }

        public readonly bool SnakeHit(int newHeadX, int newHeadY)
        {
            return GetBoardItem(newHeadX, newHeadY);
        }

        public bool Extend()
        {
            if (_actualLength < _maxLength - 1)
            {
                _desiredLength++;
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
