using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.Emit;

namespace ILCompiler.Compiler.DependencyAnalysisFramework
{
    public interface IDependencyNode
    {
        public bool Mark { get; set; }
        public void OnMarked(NodeFactory factory);

        public IList<IDependencyNode> GetStaticDependencies(DependencyNodeContext context);

        public IList<ConditionalDependency> GetConditionalStaticDependencies(DependencyNodeContext context);

        public IList<Instruction> GetInstructions(string inputFilePath, IList<string> modules);

        public IList<IDependencyNode> Dependencies { get; set; }

        public string Name { get; }

        public bool ShouldSkipEmitting(NodeFactory factory);
    }
}
