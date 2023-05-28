using ILCompiler.Common.TypeSystem.Common;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class Z80MethodCodeNode : IDependencyNode
    {
        public bool Analysed { get; set; }
        public MethodDesc Method { get; }
        public string Name => Method.FullName;

        public Z80MethodCodeNode(MethodDesc method)
        {
            Method = method;
            ParamsCount = method.Parameters().Count;
            LocalsCount = method.Body?.Variables.Count ?? 0;
            Dependencies = new List<IDependencyNode>();
        }

        public string? MethodCode { get; set; }

        public IList<IDependencyNode> Dependencies { get; set; }

        public bool CodeEmitted { get; set; }

        public bool Compiled { get; set; }

        public int ParamsCount { get; set; }
        public int LocalsCount { get; set; }
    }
}
