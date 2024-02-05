using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.PreInit;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.Importer
{
    public record ImportContext
    {
        public required BasicBlock CurrentBlock { get; init; }
        public required BasicBlock? FallThroughBlock { get; init; }
        public bool StopImporting { get; set; }
        public required MethodDesc Method { get; init; }
        public required INameMangler NameMangler { get; init; }
        public required IConfiguration Configuration { get; init; }
        public required PreinitializationManager PreinitializationManager { get; init; }
        public required CorLibModuleProvider CorLibModuleProvider { get; init; }
        public required NodeFactory NodeFactory { get; init; }
    }
}
