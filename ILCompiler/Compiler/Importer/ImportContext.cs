using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.PreInit;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.Dnlib;
using StackEntry = ILCompiler.Compiler.EvaluationStack.StackEntry;

namespace ILCompiler.Compiler.Importer
{
    public record ImportContext
    {
        public required BasicBlock CurrentBlock { get; init; }
        public BasicBlock? FallThroughBlock { get; set; } = null;
        public bool StopImporting { get; set; }
        public required MethodDesc Method { get; init; }
        public required INameMangler NameMangler { get; init; }
        public required IConfiguration Configuration { get; init; }
        public required PreinitializationManager PreinitializationManager { get; init; }
        public required CorLibModuleProvider CorLibModuleProvider { get; init; }
        public required NodeFactory NodeFactory { get; init; }
        public required DnlibModule Module { get; init; }
        public TypeDesc? Constrained { get; set; } = null;

        public InlineInfo? InlineInfo { get; set; } = null;

        public bool Inlining => InlineInfo is not null;
        
        public StackEntry GetGenericContext()
        {
            if (Method.AcquiresInstMethodTableFromThis())
            {
                var thisPtr = new LocalVariableEntry(0, VarType.Ref, 2);
                var thisType = new IndirectEntry(thisPtr, VarType.Ptr, 2);

                return thisType;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
