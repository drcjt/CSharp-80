using System;

namespace Snake
{
    public static class Program
    {
        public static void Main()
        {
            Console.Clear();

            // TODO: This doesn't do anything at the minute as initobj is unimplemented
            Random random = new Random();

            random._val = 0;
            Console.WriteLine(random.Next());
            Console.WriteLine(random.Next());
            Console.WriteLine(random.Next());
            Console.WriteLine(random.Next());
            Console.WriteLine(random.Next());
        }
    }
}
