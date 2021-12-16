using System.Collections.Generic;

namespace ILCompiler.Compiler
{
    public class SsaBuilder
    {
        public void Build(IList<BasicBlock> blocks)
        {
            var postOrder = new List<BasicBlock>();

            // Topologically sort the graph
            TopologicalSort(postOrder);
        }

        private void TopologicalSort(List<BasicBlock> postOrder)
        {
            // TODO
        }
    }
}
