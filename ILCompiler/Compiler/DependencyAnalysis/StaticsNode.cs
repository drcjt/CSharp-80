using dnlib.DotNet;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class StaticsNode : DependencyNode
    {
        public FieldDef Field { get; private set; }

        public override string Name => Field.FullName;

        public StaticsNode(FieldDef field)
        {
            Field = field;
        }
    }
}
