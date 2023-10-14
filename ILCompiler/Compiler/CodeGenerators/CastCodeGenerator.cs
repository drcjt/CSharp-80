using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class CastCodeGenerator : ICodeGenerator<CastEntry>
    {
        public void GenerateCode(CastEntry entry, CodeGeneratorContext context)
        {
            var actualType = entry.Op1.Type;
            var desiredType = entry.Type;

            var actualTypeIsIntOrUInt = actualType == VarType.Int || actualType == VarType.UInt;

            if (actualTypeIsIntOrUInt && desiredType == VarType.UShort)
            {
                context.InstructionsBuilder.Pop(HL);      // LSW
                context.InstructionsBuilder.Pop(DE);      // MSW

                context.InstructionsBuilder.Ld(DE, 0);    // clear msw

                context.InstructionsBuilder.Push(DE);     // MSW
                context.InstructionsBuilder.Push(HL);     // LSW
            }
            else if (actualTypeIsIntOrUInt && desiredType == VarType.Short)
            {
                context.InstructionsBuilder.Pop(DE);      // LSW
                context.InstructionsBuilder.Pop(HL);      // MSW

                context.InstructionsBuilder.Ld(H, D);

                context.InstructionsBuilder.Add(HL, HL);  // move sign bit into carry flag
                context.InstructionsBuilder.Sbc(HL, HL);  // hl is now 0 or FFFF

                context.InstructionsBuilder.Push(HL);     // MSW
                context.InstructionsBuilder.Push(DE);     // LSW
            }
            else if (actualTypeIsIntOrUInt && desiredType == VarType.Byte)
            {
                context.InstructionsBuilder.Pop(HL);      // LSW
                context.InstructionsBuilder.Pop(DE);      // MSW

                context.InstructionsBuilder.Ld(DE, 0);    // clear msw
                context.InstructionsBuilder.Ld(H, 0);

                context.InstructionsBuilder.Push(DE);     // MSW
                context.InstructionsBuilder.Push(HL);     // LSW
            }
            else if (actualTypeIsIntOrUInt && desiredType == VarType.SByte)
            {
                context.InstructionsBuilder.Pop(DE);      // LSW
                context.InstructionsBuilder.Pop(HL);      // MSW

                context.InstructionsBuilder.Ld(H, E);

                context.InstructionsBuilder.Add(HL, HL);  // move sign bit into carry flag
                context.InstructionsBuilder.Sbc(HL, HL);  // hl is now 0000 or FFFF
                context.InstructionsBuilder.Ld(D, L);       // D is now 00 or FF

                context.InstructionsBuilder.Push(HL);     // MSW
                context.InstructionsBuilder.Push(DE);     // LSW
            }
            else if (actualTypeIsIntOrUInt && desiredType == VarType.Ptr)
            {
                context.InstructionsBuilder.Pop(HL);      // LSW
                context.InstructionsBuilder.Pop(DE);      // MSW

                context.InstructionsBuilder.Push(HL);     // LSW
            }
            else if (actualType == VarType.ByRef && desiredType == VarType.Ptr)
            {
                // Nothing to do
            }
            else if (actualType == VarType.UInt && desiredType == VarType.Int)
            {
                // Nothing to do
            }
            else if (actualType == VarType.UShort && desiredType == VarType.Short)
            {
                // Nothing to do
            }
            else if (actualType == VarType.Ptr && (desiredType == VarType.UInt || desiredType == VarType.Int || desiredType.IsShort()))
            {
                context.InstructionsBuilder.Pop(HL);
                context.InstructionsBuilder.Ld(DE, 0);    // clear msw
                context.InstructionsBuilder.Push(DE);
                context.InstructionsBuilder.Push(HL);
            }
            else
            {
                throw new NotImplementedException($"Implicit cast from {actualType} to {desiredType} not supported");
            }
        }
    }
}
