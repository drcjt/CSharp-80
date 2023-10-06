using ILCompiler.Compiler.PreInit;
using Microsoft.Extensions.Logging;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public record DependencyNodeContext
    {
        public required ILogger<DependencyAnalyzer> Logger { get; init; }
        public required NodeFactory NodeFactory { get; init; }
        public required CorLibModuleProvider CorLibModuleProvider { get; init; }
        public required PreinitializationManager PreinitializationManager { get; init; }
    }
}
