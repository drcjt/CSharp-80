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

        int GrabTemp(VarType type, int? exactSize);

        int ParameterCount { get; }
        LocalVariableTable LocalVariableTable { get; }
        BasicBlock[] BasicBlocks { get; }

        int? ReturnBufferArgIndex { get; }
    }
}
