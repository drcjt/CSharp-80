using ILCompiler.Compiler;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.TypeSystem.Common;

namespace ILCompiler.Interfaces
{
    public interface IImporter : IPhase
    {
        public IList<BasicBlock> Import(int parameterCount, int? returnBufferArgIndex, MethodDesc method, LocalVariableTable locals, IList<EHClause> ehClauses, InlineInfo? inlineInfo = null);

        void Push(StackEntry entry);
        void ImportAppendTree(StackEntry entry, bool spill = false);
        StackEntry Pop();
        void ImportFallThrough(BasicBlock next);

        int GrabTemp(VarType type, int? exactSize);

        int ParameterCount { get; }
        LocalVariableTable LocalVariableTable { get; }
        BasicBlock[] BasicBlocks { get; }

        int? ReturnBufferArgIndex { get; }

        InlineInfo? InlineInfo { get; }

        public StackEntry InlineFetchArgument(int ilArgNum);

        public int InlineFetchLocal(int localNumber);
    }
}
