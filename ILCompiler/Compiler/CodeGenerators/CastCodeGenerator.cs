using ILCompiler.Common.TypeSystem;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class CastCodeGenerator : ICodeGenerator<CastEntry>
    {
        public void GenerateCode(CastEntry entry, CodeGeneratorContext context)
        {
            var actualKind = entry.Op1.Kind;
            var desiredType = entry.DesiredType;

            if (actualKind == StackValueKind.Int32 && desiredType == Common.TypeSystem.WellKnownType.UInt16)
            {
                context.Assembler.Pop(R16.HL);      // LSW
                context.Assembler.Pop(R16.DE);      // MSW

                context.Assembler.Ld(R16.DE, 0);    // clear msw

                context.Assembler.Push(R16.DE);     // MSW
                context.Assembler.Push(R16.HL);     // LSW
            }
            else if (actualKind == StackValueKind.Int32 && desiredType == Common.TypeSystem.WellKnownType.Int16)
            {
                context.Assembler.Pop(R16.DE);      // LSW
                context.Assembler.Pop(R16.HL);      // MSW

                context.Assembler.Ld(R8.H, R8.D);

                context.Assembler.Add(R16.HL, R16.HL);  // move sign bit into carry flag
                context.Assembler.Sbc(R16.HL, R16.HL);  // hl is now 0 or FFFF

                context.Assembler.Push(R16.HL);     // MSW
                context.Assembler.Push(R16.DE);     // LSW
            }
            else if (actualKind == StackValueKind.Int32 && desiredType == WellKnownType.Byte)
            {
                context.Assembler.Pop(R16.HL);      // LSW
                context.Assembler.Pop(R16.DE);      // MSW

                context.Assembler.Ld(R16.DE, 0);    // clear msw
                context.Assembler.Ld(R8.H, 0);

                context.Assembler.Push(R16.DE);     // MSW
                context.Assembler.Push(R16.HL);     // LSW
            }
            else if (actualKind == StackValueKind.Int32 && desiredType == WellKnownType.SByte)
            {
                context.Assembler.Pop(R16.DE);      // LSW
                context.Assembler.Pop(R16.HL);      // MSW

                context.Assembler.Ld(R8.H, R8.E);

                context.Assembler.Add(R16.HL, R16.HL);  // move sign bit into carry flag
                context.Assembler.Sbc(R16.HL, R16.HL);  // hl is now 0000 or FFFF
                context.Assembler.Ld(R8.D, R8.L);       // D is now 00 or FF

                context.Assembler.Push(R16.HL);     // MSW
                context.Assembler.Push(R16.DE);     // LSW
            }
            else if (actualKind == StackValueKind.Int32 && IsPtrType(desiredType))
            {
                context.Assembler.Pop(R16.HL);      // LSW
                context.Assembler.Pop(R16.DE);      // MSW

                context.Assembler.Push(R16.HL);     // LSW
            }
            else
            {
                throw new NotImplementedException($"Implicit cast from {actualKind} to {desiredType} not supported");
            }
        }

        private static bool IsPtrType(WellKnownType type)
        {
            return type == WellKnownType.Object ||
                   type == WellKnownType.IntPtr ||
                   type == WellKnownType.UIntPtr;
        }
    }
}
