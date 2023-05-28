using dnlib.DotNet;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    internal class ConstructedEETypeNode : IDependencyNode
    {
        public bool Analysed { get; set; }
        public bool Compiled { get; set; }

        public IList<IDependencyNode> Dependencies { get; set; }

        public TypeDef Type { get; private set; }
        public int BaseSize { get; private set; }
        public string Name => Type.FullName + " constructed";

        public ConstructedEETypeNode(TypeDef type, int baseSize)
        {
            Type = type;
            BaseSize = baseSize;
            Dependencies = new List<IDependencyNode>();
        }
    }
}
