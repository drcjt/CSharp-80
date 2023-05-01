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
                context.Emitter.Pop(HL);      // LSW
                context.Emitter.Pop(DE);      // MSW

                context.Emitter.Ld(DE, 0);    // clear msw

                context.Emitter.Push(DE);     // MSW
                context.Emitter.Push(HL);     // LSW
            }
            else if (actualTypeIsIntOrUInt && desiredType == VarType.Short)
            {
                context.Emitter.Pop(DE);      // LSW
                context.Emitter.Pop(HL);      // MSW

                context.Emitter.Ld(H, D);

                context.Emitter.Add(HL, HL);  // move sign bit into carry flag
                context.Emitter.Sbc(HL, HL);  // hl is now 0 or FFFF

                context.Emitter.Push(HL);     // MSW
                context.Emitter.Push(DE);     // LSW
            }
            else if (actualTypeIsIntOrUInt && desiredType == VarType.Byte)
            {
                context.Emitter.Pop(HL);      // LSW
                context.Emitter.Pop(DE);      // MSW

                context.Emitter.Ld(DE, 0);    // clear msw
                context.Emitter.Ld(H, 0);

                context.Emitter.Push(DE);     // MSW
                context.Emitter.Push(HL);     // LSW
            }
            else if (actualTypeIsIntOrUInt && desiredType == VarType.SByte)
            {
                context.Emitter.Pop(DE);      // LSW
                context.Emitter.Pop(HL);      // MSW

                context.Emitter.Ld(H, E);

                context.Emitter.Add(HL, HL);  // move sign bit into carry flag
                context.Emitter.Sbc(HL, HL);  // hl is now 0000 or FFFF
                context.Emitter.Ld(D, L);       // D is now 00 or FF

                context.Emitter.Push(HL);     // MSW
                context.Emitter.Push(DE);     // LSW
            }
            else if (actualTypeIsIntOrUInt && desiredType == VarType.Ptr)
            {
                context.Emitter.Pop(HL);      // LSW
                context.Emitter.Pop(DE);      // MSW

                context.Emitter.Push(HL);     // LSW
            }
            else if (actualType == VarType.ByRef && desiredType == VarType.Ptr)
            {
                // Nothing to do
            }
            else if (actualType == VarType.UInt && desiredType == VarType.Int)
            {
                // Nothing to do
            }
            else if (actualType == VarType.Ptr && (desiredType == VarType.UInt || desiredType == VarType.Int))
            {
                context.Emitter.Pop(HL);
                context.Emitter.Ld(DE, 0);    // clear msw
                context.Emitter.Push(DE);
                context.Emitter.Push(HL);
            }
            else
            {
                throw new NotImplementedException($"Implicit cast from {actualType} to {desiredType} not supported");
            }
        }
    }
}
