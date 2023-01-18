using dnlib.DotNet;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class EETypeNode : IDependencyNode
    {
        public bool Analysed { get; set; }

        public bool Compiled { get; set; }

        public TypeDef Type { get; private set; }

        public EETypeNode(TypeDef type)
        {
            Type = type;
            Dependencies = new List<IDependencyNode>();
        }

        public IList<IDependencyNode> Dependencies { get; set; }
    }
}
