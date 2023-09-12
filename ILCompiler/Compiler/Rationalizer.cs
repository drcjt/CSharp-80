using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System.Diagnostics;

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
            var node = block.FirstNode;
            while (node is PhiArg || node is PhiNode)
            {
                // Remove PhiArgs
                while (node is PhiArg)
                {
                    node = node.Next;
                }
                
                // Remove PhiNode and StoreLocalVariableEntry
                Debug.Assert(node is PhiNode);
                node = node.Next;
                Debug.Assert(node is StoreLocalVariableEntry);
                node = node.Next;

                // Remove statement
                block.Statements.RemoveAt(0);
            }
            
            block.FirstNode = node;
            if (node != null)
            {
                node.Prev = null;
            }
        }
    }
}
