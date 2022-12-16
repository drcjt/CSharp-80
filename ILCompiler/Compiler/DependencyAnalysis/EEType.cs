using dnlib.DotNet;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class EEType : IDependencyNode
    {
        public bool Analysed { get; set; }

        public TypeDef Type { get; private set; }

        public EEType(TypeDef type)
        {
            Type = type;
            Dependencies = new List<IDependencyNode>();
        }

        public IList<IDependencyNode> Dependencies { get; set; }
    }
}
