using ILCompiler.Compiler.DependencyAnalysisFramework;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public interface IMethodNode : IDependencyNode
    {
        public string GetMangledName(INameMangler nameMangler);
        public MethodDesc Method { get; }
    }
}
