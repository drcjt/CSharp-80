namespace ILCompiler.Compiler.DependencyAnalysis
{
    public record ConditionalDependency
    {
        public required IDependencyNode ThenNode { get; init; }
        public required IDependencyNode IfNode { get; init; }
    }
}
