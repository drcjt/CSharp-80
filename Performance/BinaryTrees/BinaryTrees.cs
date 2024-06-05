using System;

namespace BinaryTrees
{
    public static class BinaryTrees_2
    {
        const int minDepth = 4;

        public static unsafe int Main()
        {
            int maxDepth = Math.Max(minDepth + 2, 16);
            int stretchDepth = maxDepth + 1;

            int check = (TreeNode.bottomUpTree(stretchDepth)).itemCheck();
            int checkSum = check;
            Console.Write("stretch tree of depth ");
            Console.Write(stretchDepth);
            Console.Write("\t check: ");
            Console.WriteLine(check);

            TreeNode longLivedTree = TreeNode.bottomUpTree(maxDepth);

            for (int depth = minDepth; depth <= maxDepth; depth += 2)
            {
                int iterations = 1 << (maxDepth - depth + minDepth);

                check = 0;
                for (int i = 1; i <= iterations; i++)
                {
                    check += (TreeNode.bottomUpTree(depth)).itemCheck();
                }
                checkSum += check;

                Console.Write(iterations);
                Console.Write("\t trees of depth ");
                Console.Write(depth);
                Console.Write("\t check: ");
                Console.WriteLine(check);
            }

            check = longLivedTree.itemCheck();
            checkSum += check;

            Console.Write("long lived tree of depth ");
            Console.Write(maxDepth);
            Console.Write("\t check: ");
            Console.WriteLine(check);

            return checkSum;
        }

        struct TreeNode
        {
            class Next
            {
                public TreeNode left, right;
            }

            private Next next;

            internal static TreeNode bottomUpTree(int depth)
            {
                if (depth > 0)
                {
                    return new TreeNode(
                         bottomUpTree(depth - 1)
                       , bottomUpTree(depth - 1)
                       );
                }
                else
                {
                    return new TreeNode();
                }
            }

            TreeNode(TreeNode left, TreeNode right)
            {
                this.next = new Next();
                this.next.left = left;
                this.next.right = right;
            }

            internal int itemCheck()
            {
                // if necessary deallocate here
                if (next == null) return 1;
                else return 1 + next.left.itemCheck() + next.right.itemCheck();
            }
        }
    }
}
