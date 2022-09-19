using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler;
using ILCompiler.Compiler.EvaluationStack;

namespace ILCompiler.Interfaces
{
    public interface IILImporterProxy
    {
        void PushExpression(StackEntry entry);
        void ImportAppendTree(StackEntry entry, bool spill = false);
        StackEntry PopExpression();
        void ImportFallThrough(BasicBlock next);

        int GrabTemp(StackValueKind kind, int? exactSize, VarType type);

        int ParameterCount { get; }
        IList<LocalVariableDescriptor> LocalVariableTable { get; }
        BasicBlock[] BasicBlocks { get; }

        int? ReturnBufferArgIndex { get; }
    }
}
