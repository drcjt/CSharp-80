namespace ILCompiler.Compiler.DependencyAnalysis
{
    public abstract class DependencyNode : IDependencyNode
    {
        public bool Mark { get; set; }

        public abstract IList<IDependencyNode> GetStaticDependencies(DependencyNodeContext context);

        public abstract IList<IDependencyNode> Dependencies { get; set; }

        public abstract string Name { get; }
    }
}
