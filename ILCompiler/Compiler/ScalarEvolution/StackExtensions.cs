namespace ILCompiler.Compiler.ScalarEvolution
{
    public static class StackExtensions
    {
        public static T Bottom<T>(this Stack<T> stack, int n) => stack.ElementAt(stack.Count - n - 1);

        public static void Pop<T>(this Stack<T> stack, int n)
        {
            while (n > 0)
            {
                stack.Pop();
            }
        }

        public static void SwapTopWithBottom<T>(this Stack<T> stack, int n)
        {
            var tempStack = new Stack<T>();

            T topElement = stack.Pop();
            while (stack.Count - n > 1)
            {
                tempStack.Push(stack.Pop());
            }
            T bottomElement = stack.Pop();

            stack.Push(topElement);

            while (tempStack.Count > 0)
            {
                stack.Push(tempStack.Pop());
            }

            stack.Push(bottomElement);      
        }
    }
}
