using Microsoft.Extensions.Logging;
using System.Text;

namespace ILCompiler.Compiler.Ssa
{
    internal class SsaRenameState
    {
        internal struct StackNode
        {
            public BasicBlock Block;
            public int SsaNumber;
        }

        private readonly ILogger _logger;
        private readonly Stack<StackNode>[] _stacks;

        public SsaRenameState(ILogger logger, int lclCount)
        {
            _logger = logger;
            _stacks = new Stack<StackNode>[lclCount];
            for (int lclNumber = 0; lclNumber < lclCount; lclNumber++) 
            {
                _stacks[lclNumber] = new Stack<StackNode>();
            }
        }

        public int Top(int lclNumber)
        {
            var top = _stacks[lclNumber].Peek();
            return top.SsaNumber;
        }

        public void Push(BasicBlock block, int lclNumber, int ssaNumber) 
        {
            var stack = _stacks[lclNumber];
            if (stack.Count == 0 || stack.Peek().Block != block)
            {
                var node = new StackNode() { Block = block, SsaNumber = ssaNumber };
                stack.Push(node);
            }
            else
            {
                var top = stack.Pop();
                top.SsaNumber = ssaNumber;
                stack.Push(top);
            }

            DumpStack(lclNumber);
        }

        public void PopBlockStacks(BasicBlock block)
        {
            for (int lclNumber = 0; lclNumber < _stacks.Length; lclNumber++) 
            {
                var stack = _stacks[lclNumber];
                if (stack.Count > 0 && stack.Peek().Block == block)
                {
                    stack.Pop();
                }

                DumpStack(lclNumber);
            }
        }

        public void DumpStack(int lclNumber)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("V{0:00}: ", lclNumber);

            var stack = _stacks[lclNumber];
            var addComma = false;
            foreach (var node in stack)
            {
                sb.Append(addComma ? ", " : "");
                sb.Append($"<{node.Block.Label}, {node.SsaNumber}>");
                addComma = true;
            }

            _logger.LogDebug(sb.ToString());
        }
    }
}
