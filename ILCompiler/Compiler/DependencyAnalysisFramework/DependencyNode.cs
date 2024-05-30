using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.Emit;

namespace ILCompiler.Compiler.DependencyAnalysisFramework
{
    public abstract class DependencyNode : IDependencyNode
    {
        public bool Mark { get; set; }

        public virtual void OnMarked(NodeFactory factory) { }

        public virtual IList<IDependencyNode> GetStaticDependencies(DependencyNodeContext context) => Array.Empty<IDependencyNode>();

        public virtual IList<ConditionalDependency> GetConditionalStaticDependencies(DependencyNodeContext context) => Array.Empty<ConditionalDependency>();

        public IList<IDependencyNode> Dependencies { get; set; } = new List<IDependencyNode>();

        public abstract string Name { get; }

        public virtual IList<Instruction> GetInstructions(string inputFilePath) => Array.Empty<Instruction>();

        public virtual bool ShouldSkipEmitting(NodeFactory factory) => false;
    }
}
