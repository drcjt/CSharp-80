using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler
{
    public class Rationalizer : IRationalizer
    {
        public void Rationalize(IList<BasicBlock> blocks)
        {
            foreach (var block in blocks)
            {
                if (block.Statements.Count > 0)
                {
                    RemovePhiNodes(block);
                    RemoveCommaNodes(block);
                }
            }
        }

        private static void RemoveCommaNodes(BasicBlock block)
        {
            var node = block.FirstNode;
            StackEntry? previousNode = null;
            while (node != null)
            {
                if (node is CommaEntry && previousNode != null)
                {
                    previousNode.Next = node.Next;
                }
                previousNode = node;
                node = node.Next;
            }
        }

        private static void RemovePhiNodes(BasicBlock block)
        {
            var statement = block.Statements[0];
            while (statement is PhiNode)
            {
                // Remove PhiNode statement
                var nextStatement = statement.Next?.Next;
                block.Statements.RemoveAt(0);
                block.FirstNode = nextStatement;

                statement = nextStatement;
            }
        }
    }
}
