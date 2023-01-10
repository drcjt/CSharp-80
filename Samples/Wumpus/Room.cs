using System;

namespace Wumpus
{
    internal class Room
    {
        public int Number;

        public Room[] Neighbours;

        public bool HasPit = false;
        public bool HasBats = false;

        public Room(int roomNumber)
        {
            Number = roomNumber;
            Neighbours = new Room[3];
        }

        private bool HasDraft()
        {
            if (Neighbours[0].HasPit) return true;
            if (Neighbours[1].HasPit) return true;
            if (Neighbours[1].HasPit) return true;

            return false;
        }

        private bool HearsBats()
        {
            if (Neighbours[0].HasBats) return true;
            if (Neighbours[1].HasBats) return true;
            if (Neighbours[1].HasBats) return true;

            return false;
        }

        private bool CanSmellWumpus(Room wumpusRoom)
        {
            if (Neighbours[0] == wumpusRoom) return true;
            if (Neighbours[1] == wumpusRoom) return true;
            if (Neighbours[1] == wumpusRoom) return true;
            
            return false;
        }

        public bool IsNeighbour(int room)
        {
            if (Neighbours[0].Number == room) return true;
            if (Neighbours[1].Number == room) return true;
            if (Neighbours[2].Number == room) return true;

            return false;
        }

        public void Print(Room wumpusRoom)
        {
            Console.Write("YOU ARE IN ROOM  ");
            Console.WriteLine(Number);

            Console.Write("TUNNELS LEAD TO  ");
            Console.Write(Neighbours[0].Number);
            Console.Write("  ");
            Console.Write(Neighbours[1].Number);
            Console.Write("  ");
            Console.WriteLine(Neighbours[2].Number);

            if (HasDraft())
            {
                Console.WriteLine("I FEEL A DRAFT!");
            }
            if (HearsBats())
            {
                Console.WriteLine("BATS NEARBY!");
            }
            if (CanSmellWumpus(wumpusRoom))
            {
                Console.WriteLine("I SMELL A WUMPUS!");
            }
        }
    }
}
