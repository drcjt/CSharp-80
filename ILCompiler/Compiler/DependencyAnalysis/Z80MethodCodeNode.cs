using dnlib.DotNet;
using System.Collections.Generic;
using Z80Assembler;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class Z80MethodCodeNode
    {
        public MethodDef Method { get; }

        public Z80MethodCodeNode(MethodDef method)
        {
            Method = method;
            ParamsCount = method.Parameters.Count;
            LocalsCount = method.Body?.Variables.Count;
        }

        public IList<Instruction> MethodCode { get; set; }

        public IList<Z80MethodCodeNode> Dependencies { get; set; }

        public bool CodeEmitted { get; set; }

        public bool Compiled { get; set; }

        public int ParamsCount { get; set; }
        public int? LocalsCount { get; set; }
    }
}
