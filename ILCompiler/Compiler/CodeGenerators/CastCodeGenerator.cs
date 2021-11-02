using ILCompiler.Common.TypeSystem;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using System;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    public class CastCodeGenerator
    {
        public static void GenerateCode(CastEntry entry, Assembler assembler)
        {
            var actualKind = entry.Op1.Kind;
            var desiredType = entry.DesiredType;

            if (actualKind == StackValueKind.Int32 && desiredType == Common.TypeSystem.WellKnownType.UInt16)
            {
                assembler.Pop(R16.HL);
                assembler.Pop(R16.DE);

                assembler.Ld(R16.HL, 0);    // clear msw

                assembler.Push(R16.DE);
                assembler.Push(R16.HL);
            }
            else if (actualKind == StackValueKind.Int32 && desiredType == Common.TypeSystem.WellKnownType.Int16)
            {
                assembler.Pop(R16.HL);
                assembler.Pop(R16.DE);

                assembler.Ld(R8.H, R8.D);

                assembler.Add(R16.HL, R16.HL);  // move sign bit into carry flag
                assembler.Sbc(R16.HL, R16.HL);  // hl is now 0 or FFFF

                assembler.Push(R16.DE);
                assembler.Push(R16.HL);
            }
            else if (actualKind == StackValueKind.Int32 && desiredType == WellKnownType.Byte)
            {
                assembler.Pop(R16.HL);
                assembler.Pop(R16.DE);

                assembler.Ld(R16.HL, 0);    // clear msw
                assembler.Ld(R8.D, 0);

                assembler.Push(R16.DE);
                assembler.Push(R16.HL);
            }
            else if (actualKind == StackValueKind.Int32 && desiredType == WellKnownType.SByte)
            {
                assembler.Pop(R16.HL);
                assembler.Pop(R16.DE);

                assembler.Ld(R8.H, R8.E);

                assembler.Add(R16.HL, R16.HL);  // move sign bit into carry flag
                assembler.Sbc(R16.HL, R16.HL);  // hl is now 0000 or FFFF
                assembler.Ld(R8.D, R8.L);       // D is now 00 or FF

                assembler.Push(R16.DE);
                assembler.Push(R16.HL);
            }
            else
            {
                throw new NotImplementedException($"Implicit cast from {actualKind} to {desiredType} not supported");
            }
        }
    }
}
