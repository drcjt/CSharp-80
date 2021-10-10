using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler;
using ILCompiler.Compiler.EvaluationStack;
using System.Collections.Generic;

namespace ILCompiler.Interfaces
{
    public interface IILImporter
    {
        void PushExpression(StackEntry entry);
        void ImportAppendTree(StackEntry entry);
        StackEntry PopExpression();
        void ImportFallThrough(BasicBlock next);

        int GrabTemp(StackValueKind kind, int? exactSize); 

        int ParameterCount { get; }
        IList<LocalVariableDescriptor> LocalVariableTable { get; }
        BasicBlock[] BasicBlocks { get; }

        int? ReturnBufferArgIndex { get; }
    }
}
