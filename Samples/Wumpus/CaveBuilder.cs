namespace Wumpus
{
    internal class CaveBuilder
    {
        private Room[] _rooms = new Room[20];

        public Room[] Build()
        {
            for (var i = 0; i < _rooms.Length; i++)
            {
                _rooms[i] = new Room(i + 1);
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

            return _rooms;
        }

        private void SetRoomNeighbours(int room, int neighbour1, int neighbour2, int neighbour3)
        {
            _rooms[room - 1].Neighbours[0] = _rooms[neighbour1 - 1];
            _rooms[room - 1].Neighbours[1] = _rooms[neighbour2 - 1];
            _rooms[room - 1].Neighbours[2] = _rooms[neighbour3 - 1];
        }
    }
}
