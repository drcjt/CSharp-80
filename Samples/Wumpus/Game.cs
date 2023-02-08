using System;
using System.Diagnostics.CodeAnalysis;

namespace Wumpus
{
    /// <summary>
    /// Hunt the wumpus based on original by Gregory Yob
    /// See original mainframe basic code https://github.com/WECMuseum/hunt_the_wumpus/blob/master/WUMPUS.BAS
    /// </summary>
    internal class Game
    {
        private bool _resetGame = true;
        private bool _gameOver = false;

        private readonly Room[] _rooms;
        private Room _currentRoom;
        private Room _wumpusRoom;
        private int _arrows = 5;

        private readonly Random _rand = new();

        public Game()
        {
            _rooms = new CaveBuilder().Build();

            ResetGame();
        }

        public void Play()
        {
            while (true)
            {
                while (!_resetGame)
                {
                    RunTheGame();
                }
                ResetGame();
            }
        }

        private void RunTheGame()
        {
            Console.WriteLine("HUNT THE WUMPUS");
            Console.WriteLine();

            _arrows = 5;
            _gameOver = false;
            while (!_gameOver)
            {
                _currentRoom.Print(_wumpusRoom);

                Console.Write("SHOOT OR MOVE (S-M)? ");
                char ch = ReadChar();

                if (ch == 'M')
                {
                    Move(GetRoomNumber());
                }
                else if (ch == 'S')
                {
                    Shoot();
                }

                if (!_gameOver)
                {
                    MoveWumpus();
                }
            }
        }

        private void Shoot()
        {
            int numberOfShots;
            do
            {
                Console.Write("NO. OF ROOMS(1-5)? ");
                numberOfShots = int.Parse(Console.ReadLine());
            } while (numberOfShots < 1 || numberOfShots > 5);

            var roomNumbersToShoot = new int[numberOfShots];
            for (int shot = 0; shot < numberOfShots; shot++)
            {
                bool validRoom;
                do
                {
                    validRoom = true;
                    Console.Write("ROOM #? ");
                    roomNumbersToShoot[shot] = int.Parse(Console.ReadLine());

                    if (shot > 2 && roomNumbersToShoot[shot] != roomNumbersToShoot[shot - 2])
                    {
                        Console.WriteLine("ARROWS AREN'T THAT CROOKED - TRY ANOTHER ROOM");
                        validRoom = false;
                    }
                } while (!validRoom);
            }

            for (var shot = 0; shot < numberOfShots && _arrows > 0; shot++)
            {
                var roomNumberToShoot = roomNumbersToShoot[shot];
                if (!_currentRoom.IsNeighbour(roomNumberToShoot))
                {
                    // Pick random neighbour
                    roomNumberToShoot = _currentRoom.Neighbours[_rand.Next(3)].Number;
                }

                if (ShotWumpus(roomNumberToShoot))
                {
                    Console.WriteLine("AHA! YOU GOT THE WUMPUS!");
                    Console.WriteLine("HEE HEE HEE - THE WUMPUS'LL GETCHA NEXT TIME!!");
                    PlayAgain();
                    return;
                }
                else if (ShotYourself(roomNumberToShoot))
                {
                    Console.WriteLine("OUCH! ARROW GOT YOU!");
                    Console.WriteLine("HA HA HA - YOU LOSE!");
                    PlayAgain();
                    return;
                }

                _arrows--;
            }

            Console.WriteLine("MISSED");

            MoveWumpus();

            // Ammo check
            if (_arrows == 0)
            {
                Console.WriteLine("HA HA HA - YOU LOSE!");
                PlayAgain();
            }
        }

        private bool ShotYourself(int roomNumber) => _currentRoom.Number == roomNumber;
        private bool ShotWumpus(int roomNumber) => _wumpusRoom.Number == roomNumber;

        private int GetRoomNumber()
        {
            var roomNumber = -1;
            while (roomNumber == -1)
            {
                Console.Write("WHERE TO? ");
                roomNumber = int.Parse(Console.ReadLine());
                roomNumber = _currentRoom.IsNeighbour(roomNumber) ? roomNumber : -1;

                if (roomNumber == -1)
                {
                    Console.Write("NOT POSSIBLE -");
                }
            }
            return roomNumber;
        }

        private static char ReadChar()
        {
            // TODO: This might result in out of memory due to ReadLine allocating on the heap
            string s = Console.ReadLine();
            if (s.Length > 0)
            {
                return s[0];
            }

            return ' ';
        }

        private void MoveWumpus()
        {
            var neighbour = _rand.Next(4);
            if (neighbour != 3)
            {
                _wumpusRoom = _wumpusRoom.Neighbours[neighbour];
            }

            if (_wumpusRoom == _currentRoom)
            {
                Console.WriteLine("TSK TSK TSK - WUMPUS GOT YOU");
                PlayAgain();
            }
        }

        private void Move(int roomNumber)
        {
            _currentRoom = _rooms[roomNumber - 1];

            if (_currentRoom == _wumpusRoom)
            {
                Console.WriteLine("... OOPS! BUMPED A WUMPUS");
                MoveWumpus();
                if (_gameOver)
                {
                    return;
                }
            }
            if (_currentRoom.HasPit)
            {
                Console.WriteLine("YYYIIIIEEEE . . . FELL IN PIT");
                Console.WriteLine("HA HA HA - YOU LOSE!");
                PlayAgain();
                return;
            }
            if (_currentRoom.HasBats)
            {
                Console.WriteLine("ZAP--SUPER BAT SNATCH! ELSEWHEREVILLE FOR YOU!");
                int move = RandomRoom();
                Move(move);
            }
        }

        private void PlayAgain()
        {
            _gameOver = true;

            Console.Write("SAME SET-UP (Y-N)? ");
            var ch = ReadChar();
            if (ch != 'Y')
            {
                _resetGame = false;
            }
        }

        private int RandomRoom() => _rand.Next(20);

        [MemberNotNull(nameof(_wumpusRoom), nameof(_currentRoom))]
        private void ResetGame()
        {
            for (var i = 0; i < _rooms.Length; i++)
            {
                _rooms[i].HasBats = false;
                _rooms[i].HasPit = false;
            }
            
            _wumpusRoom = _rooms[RandomRoom()];

            _rooms[RandomRoom()].HasPit = true;
            _rooms[RandomRoom()].HasPit = true;

            _rooms[RandomRoom()].HasBats = true;
            _rooms[RandomRoom()].HasBats = true;

            do
            {
                _currentRoom = _rooms[RandomRoom()];
            } while (_currentRoom.HasPit || _currentRoom == _wumpusRoom);

            _resetGame = false;
        }
    }
}
