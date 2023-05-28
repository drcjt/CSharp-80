using dnlib.DotNet;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class StaticsNode : IDependencyNode
    {
        public bool Analysed { get; set; }

        public bool Compiled { get; set; }

        public FieldDef Field { get; private set; }

        public string Name => Field.FullName;

        public StaticsNode(FieldDef field)
        {
            Field = field;
            Dependencies = new List<IDependencyNode>();
        }

        public IList<IDependencyNode> Dependencies { get; set; }
    }
}
