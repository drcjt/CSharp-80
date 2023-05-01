using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class CallCodeGenerator : ICodeGenerator<CallEntry>
    {
        public void GenerateCode(CallEntry entry, CodeGeneratorContext context)
        {
            if (entry.IsInternalCall)
            {
                if (entry.Arguments.Count > 0)
                {
                    // Pass first argument in HL for 2 bytes or less
                    // and in HL, DE for larger datatypes
                    var argument = entry.Arguments[0];
                    context.Emitter.Pop(HL);
                    if (argument.Type == VarType.Int || argument.Type == VarType.UInt)
                    {
                        context.Emitter.Pop(DE);
                    }
                }
            }
            context.Emitter.Call(entry.TargetMethod);
        }
    }
}
