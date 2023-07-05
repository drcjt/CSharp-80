using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class CallCodeGenerator : ICodeGenerator<CallEntry>
    {
        public void GenerateCode(CallEntry entry, CodeGeneratorContext context)
        {
            if (entry.IsInternalCall && entry.Arguments.Count > 0)
            {
                // Pass last argument in HL for Ptr type
                // and in HL, DE otherwise
                var argument = entry.Arguments[entry.Arguments.Count - 1];
                context.Emitter.Pop(HL);

                if (argument.Type != VarType.Ptr && argument.Type != VarType.Ref) // TODO: should this also check for ByRef??
                {
                    context.Emitter.Pop(DE);
                }
            }
            context.Emitter.Call(entry.TargetMethod);
        }
    }
}
