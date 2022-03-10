using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.Lowerings;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler
{
    public class Lowering : ILowering, IGenericStackEntryVisitor
    {
        private readonly ILoweringFactory _loweringFactory;

        public Lowering(ILoweringFactory loweringFactory)
        {
            _loweringFactory = loweringFactory;
        }

        public void Run(IList<BasicBlock> blocks)
        {
            foreach (var block in blocks)
            {
                LowerBlock(block);
            }
        }

        private StackEntry? _nextNode;

        public void Visit<T>(T entry) where T : StackEntry
        {
            var lowering = _loweringFactory.GetLowering<T>();
            if (lowering != null)
            {
                _nextNode = lowering.Lower(entry);
            }
        }

        private void LowerBlock(BasicBlock block)
        {
            var node = block.FirstNode;
            while (node != null)
            {
                node = LowerNode(node);
            }
        }

        private StackEntry? LowerNode(StackEntry node)
        {
            var visitorAdapter = new GenericStackEntryAdapter(this);

            _nextNode = null;
            node.Accept(visitorAdapter);
            if (_nextNode != null)
            {
                return _nextNode;
            }

            return node.Next;
        }
    }
}
