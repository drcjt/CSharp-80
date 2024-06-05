using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.IL;
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

            
            // Emit conditional call to ThrowHelpers.ThrowIndexOutOfRangeException
            var throwHelperMethod = context.CorLibModuleProvider.GetHelperEntryPoint("ThrowHelpers", "ThrowIndexOutOfRangeException");
            var mangledThrowHelperMethod = context.NameMangler.GetMangledMethodName(throwHelperMethod);

            context.InstructionsBuilder.Call(Condition.C, mangledThrowHelperMethod);
        }
    }
}
