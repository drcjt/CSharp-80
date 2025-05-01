using ILCompiler.Compiler.EvaluationStack;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Collections;

namespace ILCompiler.Compiler.LinearIR
{
    public class Range : IEnumerable<StackEntry?>
    {
        public StackEntry? FirstNode { get; set; }
        public StackEntry? LastNode { get; set; }

        public Range()
        {
        }

        public Range(StackEntry? firstNode, StackEntry? lastNode)
        {
            FirstNode = firstNode;
            LastNode = lastNode;
        }

        /// <summary>
        /// Try to find the use for a given node
        /// </summary>
        /// <param name="node">node to find the corresponding use</param>
        /// <param name="use">use of the corresponding node if any</param>
        /// <returns></returns>
        public bool TryGetUse(StackEntry node, [NotNullWhen(returnValue: true)] out Use? use)
        {
            if (node != LastNode)
            {
                var range = new Range(node.Next, LastNode);

                foreach (var n in range)
                {
                    if (n != null && n.TryGetUse(node, out Edge<StackEntry>? edge))
                    {
                        use = new Use(this, edge, n);
                        return true;
                    }
                }
            }

            use = null;
            return false;
        }

        public void Remove(StackEntry node)
        {
            var prev = node.Prev;
            var next = node.Next;

            if (prev != null)
            {
                prev.Next = next;
            }
            else
            {
                FirstNode = next;
            }

            if (next != null)
            {
                next.Prev = prev;
            }
            else
            {
                LastNode = prev;
            }

            node.Prev = null;
            node.Next = null;
        }

        public void InsertBefore(StackEntry insertion, Range range)
        {
            Debug.Assert(range.FirstNode != null);
            Debug.Assert(range.LastNode != null);
            FinishInsertionBefore(insertion, range.FirstNode, range.LastNode);
        }

        public void InsertAtEnd(Range range)
        {
            InsertAfter(LastNode, range);
        }

        private void FinishInsertionBefore(StackEntry insertionPoint, StackEntry first, StackEntry last)
        {
            if (insertionPoint == null)
            {
                if (FirstNode == null)
                {
                    FirstNode = first;
                }
                else
                {
                    LastNode!.Next = first;
                    first.Next = LastNode;
                }
                LastNode = last;
            }
            else
            {
                first.Prev = insertionPoint.Prev;
                if (first.Prev == null)
                {
                    FirstNode = first;
                }
                else
                {
                    first.Prev.Next = first;
                }
                last.Next = insertionPoint;
                insertionPoint.Prev = last;
            }
        }

        public void InsertAfter(StackEntry insertionPoint, StackEntry node1, StackEntry node2)
        {
            node1.Next = node2;
            node2.Prev = node1;

            FinishInsertionAfter(insertionPoint, node1, node2);
        }

        public void InsertAfter(StackEntry insertionPoint, StackEntry node1, StackEntry node2, StackEntry node3)
        {
            node1.Next = node2;
            node2.Prev = node1;
            node2.Next = node3;
            node3.Prev = node2;

            FinishInsertionAfter(insertionPoint, node1, node3);
        }

        public void InsertAfter(StackEntry? insertion, Range range)
        {
            Debug.Assert(!range.IsEmpty);
            FinishInsertionAfter(insertion, range.FirstNode!, range.LastNode!);
        }

        private void FinishInsertionAfter(StackEntry? insertionPoint, StackEntry first, StackEntry last)
        {
            if (insertionPoint == null)
            {
                if (LastNode == null)
                {
                    LastNode = last;
                }
                else
                {
                    FirstNode!.Prev = last;
                    last.Next = FirstNode;
                }
                FirstNode = first;
            }
            else
            {
                last.Next = insertionPoint.Next;
                if (last.Next == null)
                {
                    LastNode = last;
                }
                else
                {
                    last.Next.Prev = last;
                }

                first.Prev = insertionPoint;
                insertionPoint.Next = first;
            }
        }

        public IEnumerator<StackEntry?> GetEnumerator()
        {
            var current = FirstNode;
            do
            {
                yield return current;
                current = current?.Next;
            } while (current != LastNode);
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public bool IsEmpty => FirstNode == null || LastNode == null;
    }
}