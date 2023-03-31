using ILCompiler.Compiler.Emit;
using ILCompiler.Compiler.EvaluationStack;

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
                context.Emitter.Pop(R16.HL);      // LSW
                context.Emitter.Pop(R16.DE);      // MSW

                context.Emitter.Ld(R16.DE, 0);    // clear msw

                context.Emitter.Push(R16.DE);     // MSW
                context.Emitter.Push(R16.HL);     // LSW
            }
            else if (actualTypeIsIntOrUInt && desiredType == VarType.Short)
            {
                context.Emitter.Pop(R16.DE);      // LSW
                context.Emitter.Pop(R16.HL);      // MSW

                context.Emitter.Ld(R8.H, R8.D);

                context.Emitter.Add(R16.HL, R16.HL);  // move sign bit into carry flag
                context.Emitter.Sbc(R16.HL, R16.HL);  // hl is now 0 or FFFF

                context.Emitter.Push(R16.HL);     // MSW
                context.Emitter.Push(R16.DE);     // LSW
            }
            else if (actualTypeIsIntOrUInt && desiredType == VarType.Byte)
            {
                context.Emitter.Pop(R16.HL);      // LSW
                context.Emitter.Pop(R16.DE);      // MSW

                context.Emitter.Ld(R16.DE, 0);    // clear msw
                context.Emitter.Ld(R8.H, 0);

                context.Emitter.Push(R16.DE);     // MSW
                context.Emitter.Push(R16.HL);     // LSW
            }
            else if (actualTypeIsIntOrUInt && desiredType == VarType.SByte)
            {
                context.Emitter.Pop(R16.DE);      // LSW
                context.Emitter.Pop(R16.HL);      // MSW

                context.Emitter.Ld(R8.H, R8.E);

                context.Emitter.Add(R16.HL, R16.HL);  // move sign bit into carry flag
                context.Emitter.Sbc(R16.HL, R16.HL);  // hl is now 0000 or FFFF
                context.Emitter.Ld(R8.D, R8.L);       // D is now 00 or FF

                context.Emitter.Push(R16.HL);     // MSW
                context.Emitter.Push(R16.DE);     // LSW
            }
            else if (actualTypeIsIntOrUInt && desiredType == VarType.Ptr)
            {
                context.Emitter.Pop(R16.HL);      // LSW
                context.Emitter.Pop(R16.DE);      // MSW

                context.Emitter.Push(R16.HL);     // LSW
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
                context.Emitter.Pop(R16.HL);
                context.Emitter.Ld(R16.DE, 0);    // clear msw
                context.Emitter.Push(R16.DE);
                context.Emitter.Push(R16.HL);
            }
            else
            {
                throw new NotImplementedException($"Implicit cast from {actualType} to {desiredType} not supported");
            }
        }
    }
}
