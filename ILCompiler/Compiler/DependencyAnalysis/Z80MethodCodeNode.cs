using dnlib.DotNet;
using ILCompiler.z80;
using System.Collections.Generic;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class Z80MethodCodeNode
    {
        public MethodDef Method { get; }

        public Z80MethodCodeNode(MethodDef method)
        {
            Method = method;
        }

        public IList<Instruction> MethodCode { get; set; } = new List<Instruction>();

        public IList<Z80MethodCodeNode> Dependencies { get; set; }
        
        public bool CodeEmitted { get; set; }
    }
}
