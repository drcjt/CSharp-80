using dnlib.DotNet;
using Z80Assembler;
using ILCompiler.Common.TypeSystem.Common;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class Z80MethodCodeNode
    {
        public MethodDesc Method { get; }

        public Z80MethodCodeNode(MethodDesc method)
        {
            Method = method;
            ParamsCount = method.Parameters.Count;
            LocalsCount = method.Body?.Variables.Count ?? 0;
        }

        public IList<Instruction>? MethodCode { get; set; }

        public IList<Z80MethodCodeNode>? Dependencies { get; set; }

        public bool CodeEmitted { get; set; }

        public bool Compiled { get; set; }

        public int ParamsCount { get; set; }
        public int LocalsCount { get; set; }
    }
}
