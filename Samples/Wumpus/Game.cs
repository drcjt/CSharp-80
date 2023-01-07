using System;

namespace Wumpus
{
    internal class Game
    {
        // https://github.com/ChrisYGarcia/Hunt-The-Wumpus/tree/master/Wumpus

        Room[] _rooms = new Room[21];
        Room _currentRoom;
        Random _rand = new Random();

        public Game() 
        {
            InitialiseRooms();
            ResetGame();
        }

        public void Run()
        {
            Console.WriteLine("Welcome to Hunt the Wumpus.");
            Console.WriteLine();

            while (true)
            {
                _currentRoom.Print();

                var inputLine = Console.ReadLine();
                var roomNumber = int.Parse(inputLine);

                if (!_currentRoom.IsNeighbour(roomNumber)) 
                {
                    Console.WriteLine("Invalid room");
                    continue;
                }

                Move(roomNumber);
            }
        }

        private void Move(int roomNumber)
        {
            _currentRoom = _rooms[roomNumber];

            /*
            Console.Write("HasWumpus="); Console.WriteLine(_currentRoom.HasWumpus ? "True" : "False");
            Console.Write("HasPit="); Console.WriteLine(_currentRoom.HasPit ? "True" : "False");
            */

            /*
            if (_currentRoom.HasWumpus)
            {
                Console.WriteLine("Oh no! You have wandered into the lair of the lurking wumpus");
                Console.WriteLine("***YOU HAVE DIED***");
            }
            */
            /*
            if (_currentRoom.HasPit)
            {
                //Console.WriteLine("AAAAAAAAAAAAaaaaaaaaaaaa............");
                //Console.WriteLine("*THUD*");
                Console.WriteLine("You have fallen into a pit and died");
            }
            var hasBats = _currentRoom.HasBats;
            if (hasBats)
            {
                Console.WriteLine("*FLAP* *FLAP* *FLAP* Bats fly you elsewhere in the caves...");
                int move = NextRand();
                Move(move);
            }
            */
        }

        private void InitialiseRooms()
        {
            for (var i = 0; i < _rooms.Length; i++) 
            {
                _rooms[i] = new Room(i);
            }

            SetRoomNeighbours(1, 2, 5, 8);
            SetRoomNeighbours(2, 1, 3, 10);
            SetRoomNeighbours(3, 2, 4, 12);
            SetRoomNeighbours(4, 3, 5, 14);
            SetRoomNeighbours(5, 1, 4, 6);
            SetRoomNeighbours(6, 5, 7, 15);
            SetRoomNeighbours(7, 6, 8, 17);
            SetRoomNeighbours(8, 1, 7, 9);
            SetRoomNeighbours(9, 8, 10, 18);
            SetRoomNeighbours(10, 2, 9, 11);
            SetRoomNeighbours(11, 10, 12, 19);
            SetRoomNeighbours(12, 3, 11, 13);
            SetRoomNeighbours(13, 12, 14, 20);
            SetRoomNeighbours(14, 4, 13, 15);
            SetRoomNeighbours(15, 6, 14, 16);
            SetRoomNeighbours(16, 15, 17, 20);
            SetRoomNeighbours(17, 7, 16, 18);
            SetRoomNeighbours(18, 9, 17, 19);
            SetRoomNeighbours(19, 11, 18, 20);
            SetRoomNeighbours(20, 13, 16, 19);
        }

        private int NextRand() => _rand.Next(19) + 1;

        private void ResetGame()
        {
            int r = NextRand();
            _rooms[r].HasWumpus = true;

            r = NextRand();
            _rooms[r].HasPit = true;
            r = NextRand();
            _rooms[r].HasPit = true;

            r = NextRand();
            _rooms[r].HasBats = true;
            r = NextRand();
            _rooms[r].HasBats = true;

            do
            {
                r = NextRand();
                _currentRoom = _rooms[r];
            } while (_currentRoom.HasPit || _currentRoom.HasWumpus);
        }

        private void SetRoomNeighbours(int room, int neighbour1, int neighbour2, int neighbour3)
        {           
            _rooms[room].Neighbours[0] = _rooms[neighbour1];
            _rooms[room].Neighbours[1] = _rooms[neighbour2];
            _rooms[room].Neighbours[2] = _rooms[neighbour3];
        }

        private void DumpGame()
        {
            for (var i = 1; i < _rooms.Length; i++)
            {
                Console.Write("Room "); Console.Write(i); Console.Write(":");
                if (_rooms[i].HasBats) Console.Write(" bats");
                if (_rooms[i].HasPit) Console.Write(" pit");
                if (_rooms[i].HasWumpus) Console.Write(" wumpus");
                Console.Write(". ");
            }
        }
    }
}
