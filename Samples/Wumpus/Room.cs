using System;

namespace Wumpus
{
    internal class Room
    {
        public int Number;

        public Room[] Neighbours;

        public bool HasPit;
        public bool HasBats;
        public bool HasWumpus;

        public Room(int roomNumber)
        {
            Number = roomNumber;
            Neighbours = new Room[3];

            /*
            HasPit = false;
            HasBats = false;
            HasWumpus = false;
            */
        }

        private bool HasDraft()
        {
            for (var i = 0; i < Neighbours.Length; i++) 
            {
                var neighbour = Neighbours[i];
                if (neighbour.HasPit) return true;
            }

            return false;
        }

        private bool HearsBats()
        {
            for (var i = 0; i < Neighbours.Length; i++)
            {
                var neighbour = Neighbours[i];
                if (neighbour.HasBats) return true;
            }

            return false;
        }

        private bool CanSmellWumpus()
        {
            for (var i = 0; i < Neighbours.Length; i++)
            {
                var neighbour = Neighbours[i];
                if (neighbour.HasWumpus) return true;
            }

            return false;
        }


        public bool IsNeighbour(int room)
        {
            for (var i = 0; i < Neighbours.Length; i++)
            {
                var neighbour = Neighbours[i];
                if (neighbour.Number == room) return true;
            }

            return false;
        }

        public void Print()
        {
            Console.Write("You are in room ");
            Console.WriteLine(Number);

            Console.Write("There are passages to rooms ");
            Console.Write(Neighbours[0].Number);
            Console.Write(", ");
            Console.Write(Neighbours[1].Number);
            Console.Write(", and ");
            Console.WriteLine(Neighbours[2].Number);

            if (HasDraft())
            {
                Console.WriteLine("~You can feel a draft~");
            }
            if (HearsBats())
            {
                Console.WriteLine("*Squeak* *Squeak* There must be bats nearby.");
            }
            if (CanSmellWumpus())
            {
                Console.WriteLine("I SMELL A WUMPUS!");
            }


        }
    }
}
