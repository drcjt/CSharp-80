using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class BoundsCheckCodeGenerator : ICodeGenerator<BoundsCheck>
    {
        public void GenerateCode(BoundsCheck entry, CodeGeneratorContext context)
        {
            // Stack will contain Index followed by array length
            // just need to pop them off here, compare and fail if index is beyond end of array

            context.Emitter.Pop(HL);        // Array Length
            context.Emitter.Dec(HL);        // Make 0 based

            context.Emitter.Pop(DE);        // Index to validate

            context.Emitter.And(A);         // Clear carry flag
            context.Emitter.Sbc(HL, DE);    // Calculate Array Length - Index

            var endLabel = context.NameMangler.GetUniqueName();

            context.Emitter.Jp(Condition.NC, endLabel);

            // Code to display Index Out of Range message and fail fast

            context.Emitter.Ld(HL, "INDEX_OUT_OF_RANGE_MSG - 2");
            context.Emitter.Call("PRINT");
            context.Emitter.Jp("EXIT");

            // Index was valid
            context.Emitter.CreateLabel(endLabel);
        }
    }
}
