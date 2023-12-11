namespace ILCompiler.Compiler.Helpers
{
    public class DfsReversePostorderHelper
    {
        private sealed class DfsBlockEntry
        {
            public BasicBlock Block { get; init; }
            private IEnumerator<BasicBlock>? _enumerator;

            public DfsBlockEntry(BasicBlock block)
            {
                Block = block;
            }

            public BasicBlock? GetNextSuccessor()
            {
                if (Block.Successors == null)
                {
                    return null;
                }

                if (_enumerator == null)
                {
                    _enumerator = Block.Successors.GetEnumerator();
                }

                if (_enumerator.MoveNext())
                {
                    return _enumerator.Current;
                }
                return null;
            }
        }

        public static IEnumerable<BasicBlock> CreateDfsReversePostorder(IList<BasicBlock> blocks)
        {
            var stack = new Stack<DfsBlockEntry>();
            var visited = new HashSet<BasicBlock>();
            stack.Push(new DfsBlockEntry(blocks[0]));

            var postOrderTraversal = new List<BasicBlock>();

            while (stack.Count > 0)
            {
                var current = stack.Peek();
                var successor = current.GetNextSuccessor();

                if (successor == null)
                {
                    postOrderTraversal.Add(current.Block);
                    stack.Pop();
                }
                else if (!visited.Contains(successor))
                {
                    stack.Push(new DfsBlockEntry(successor));
                    visited.Add(successor);
                }
            }

            return postOrderTraversal.AsEnumerable().Reverse();
        }
    }
}