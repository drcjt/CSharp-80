namespace ILCompiler.Compiler.DependencyAnalysis
{
    public interface IDependencyNode
    {
        public bool Analysed { get; set; }
        public IList<IDependencyNode> Dependencies { get; set; }
    }
}
