using ILCompiler.Compiler;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.Inlining;
using ILCompiler.Compiler.OpcodeImporters;
using ILCompiler.Compiler.PreInit;
using ILCompiler.TypeSystem.Common;
using StackEntry = ILCompiler.Compiler.EvaluationStack.StackEntry;

namespace ILCompiler.Interfaces
{
    public interface IImporter : IPhase
    {
        public CodeFolder CodeFolder { get; }
        public INameMangler NameMangler { get; }
        public IConfiguration Configuration { get; }
        public PreinitializationManager PreinitializationManager { get; }
        public NodeFactory NodeFactory { get; }

        public IList<BasicBlock> Import(int parameterCount, int? returnBufferArgIndex, MethodDesc method, LocalVariableTable locals, IList<EHClause> ehClauses, InlineInfo? inlineInfo = null);

        void Push(StackEntry entry);
        StackEntry Pop();

        void ImportAppendTree(StackEntry entry, bool spill = false);
        void ImportFallThrough(BasicBlock next);

        int GrabTemp(VarType type, int? exactSize);

        int ParameterCount { get; }
        LocalVariableTable LocalVariableTable { get; }
        BasicBlock[] BasicBlocks { get; }
        public BasicBlock? FallThroughBlock { get; }

        int? ReturnBufferArgIndex { get; }

        InlineInfo? InlineInfo { get; }
        public bool Inlining { get; }
        public StackEntry InlineFetchArgument(int ilArgNum);
        public int InlineFetchLocal(int localNumber);

        public MethodDesc Method { get; }

        public StackEntry GetGenericContext();

        public TypeDesc? Constrained { get; set; }

        public bool StopImporting { get; set; }

        public int MapIlArgNum(int ilArgNum);

        public StackEntry StoreStruct(StackEntry node);

        public StackEntry GetNodeAddress(StackEntry value);

        public StackEntry NewTempStore(int tempNumber, StackEntry value);
    }
}
