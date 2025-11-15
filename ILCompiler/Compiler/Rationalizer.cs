using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using System.Diagnostics;

namespace ILCompiler.Compiler
{
    public class Rationalizer : IRationalizer
    {
        public void Rationalize(MethodCompiler compiler)
        {
            foreach (var block in compiler.Blocks)
            {
                // Link IR in statements together
                foreach (var statement in block.Statements)
                {
                    var firstNode = statement.TreeList[0];
                    var lastNode = statement.TreeList[^1];

                    block.InsertAtEnd(new LinearIR.Range(firstNode, lastNode));
                }
               
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
                else
                {
                    previousNode = node;
                }
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
