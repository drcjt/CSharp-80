namespace System.Collections.Tests
{
    internal static class Helpers
    {
        public static Stack CreateIntStack(int count, int start = 0)
        {
            var stack = new Stack(100);

            for (int i = start; i < start + count; i++)
            {
                stack.Push(i);
            }

            return stack;
        }
    }
}
