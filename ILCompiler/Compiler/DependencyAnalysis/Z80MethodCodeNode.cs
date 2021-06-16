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

        public IList<Instruction> MethodCode { get; set; }

        public IList<MethodDef> DependsOn { get; set; }

        public bool Compiled { get; set; }
    }
}
