using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILCompiler.Compiler
{
    public class Flowgraph
    {
        public void SetBlockOrder(IList<BasicBlock> blocks)
        {
            // Put code here to set execution order within the basic blocks

            // canonical post order traversal of the HIR tree

            // Will iterate the StackEntrys in each basic block and traverse the statement tree
            // setting the Next property on each stack entry to indicate the true execution order within the block
        }
    }
}
