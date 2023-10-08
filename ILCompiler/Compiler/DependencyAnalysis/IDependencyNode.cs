namespace ILCompiler.Compiler.DependencyAnalysis
{
    public interface IDependencyNode
    {
        public bool Mark { get; set; }

        public IList<IDependencyNode> GetStaticDependencies(DependencyNodeContext context);

        public IList<ConditionalDependency> GetConditionalStaticDependencies(DependencyNodeContext context);

        public IList<IDependencyNode> Dependencies { get; set; }

        public string Name { get; }
    }
}
