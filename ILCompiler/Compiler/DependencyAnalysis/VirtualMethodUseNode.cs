using ILCompiler.Common.TypeSystem.Common;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class VirtualMethodUseNode : DependencyNode
    {
        public MethodDesc Method { get; }
        public override IList<IDependencyNode> Dependencies { get; set; }

        public override string Name => Method.FullName;

        public VirtualMethodUseNode(MethodDesc method)
        {
            Method = method;
            Dependencies = new List<IDependencyNode>();
        }

        public override IList<IDependencyNode> GetStaticDependencies(DependencyNodeContext context) => Dependencies;
        public override IList<ConditionalDependency> GetConditionalStaticDependencies(DependencyNodeContext context) => new List<ConditionalDependency>();

    }
}
