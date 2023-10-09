namespace ILCompiler.Compiler.DependencyAnalysis
{
    public abstract class DependencyNode : IDependencyNode
    {
        public bool Mark { get; set; }
        public virtual void OnMarked(NodeFactory factory)
        {
        }

        public virtual IList<IDependencyNode> GetStaticDependencies(DependencyNodeContext context) => new List<IDependencyNode>();

        public virtual IList<ConditionalDependency> GetConditionalStaticDependencies(DependencyNodeContext context) => new List<ConditionalDependency>();

        public IList<IDependencyNode> Dependencies { get; set; } = new List<IDependencyNode>();

        public abstract string Name { get; }
    }
}
