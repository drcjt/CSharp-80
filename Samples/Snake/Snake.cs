using System;
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

        public int Length;

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
            Length = 1;

            _snakeXs = snakeXs;
            _snakeYs = snakeYs;

            _maxLength = length;

            _snakeXs[0] = x;
            _snakeYs[0] = y;
        }

        private static int WrapAround(int coordinate, int max)
        {
            coordinate %= max + 1;
            return (coordinate < 0) ? max : coordinate;
        }


        public unsafe bool Update()
        {
            var newHeadX = HeadX;
            var newHeadY = HeadY;
            switch (_direction)
            {
                case Direction.Left: newHeadX--; break;
                case Direction.Right: newHeadX++; break;
                case Direction.Up: newHeadY--; break;
                case Direction.Down: newHeadY++; break;
            }

            newHeadX = WrapAround(newHeadX, Graphics.ScreenWidth / 2);
            newHeadY = WrapAround(newHeadY, Graphics.ScreenHeight);

            // TODO: This slows down as the length of the snake increases
            // need to look at optimisations in codegen to improve the
            // speed of the generated code
            for (int i = 0; i < Length; i++)
            {
                var index = (_snakeHead - i) % _maxLength;
                if (_snakeXs[index] == newHeadX &&
                    _snakeYs[index] == newHeadY)
                {
                    // Hit snake with itself
                    return false;
                }
            }

            // Add new head
            _snakeHead = (_snakeHead + 1) % _maxLength;
            _snakeXs[_snakeHead] = newHeadX;
            _snakeYs[_snakeHead] = newHeadY;

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

            return true;
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
