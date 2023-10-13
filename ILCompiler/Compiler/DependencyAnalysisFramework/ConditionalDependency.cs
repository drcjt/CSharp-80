namespace ILCompiler.Compiler.DependencyAnalysisFramework
{
    public record ConditionalDependency
    {
        public required IDependencyNode ThenNode { get; init; }
        public required IDependencyNode ThenParent { get; init; }
        public required IDependencyNode IfNode { get; init; }
    }
}
