using ILCompiler.Interfaces;

namespace ILCompiler.Compiler
{
    public class Morpher : IMorpher
    {
        public void Morph(IList<BasicBlock> blocks)
        {
            // TODO: Add Morphing code here
            // fgMorphBlocks -> fgMorphStmts -> fgMorphTree -> fgMorphSmpOp -> fgMorphArrayIndex
        }
    }
}
