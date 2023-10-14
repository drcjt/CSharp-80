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

            context.InstructionsBuilder.Pop(HL);        // Array Length
            context.InstructionsBuilder.Dec(HL);        // Make 0 based

            context.InstructionsBuilder.Pop(DE);        // Index to validate

            context.InstructionsBuilder.And(A);         // Clear carry flag
            context.InstructionsBuilder.Sbc(HL, DE);    // Calculate Array Length - Index

            var endLabel = context.NameMangler.GetUniqueName();

            context.InstructionsBuilder.Jp(Condition.NC, endLabel);

            // Code to display Index Out of Range message and fail fast

            context.InstructionsBuilder.Ld(HL, "INDEX_OUT_OF_RANGE_MSG - 2");
            context.InstructionsBuilder.Call("PRINT");
            context.InstructionsBuilder.Jp("EXIT");

            // Index was valid
            context.InstructionsBuilder.Label(endLabel);
        }
    }
}
