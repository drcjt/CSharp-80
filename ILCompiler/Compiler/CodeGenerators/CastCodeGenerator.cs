using ILCompiler.Compiler.EvaluationStack;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal class CastCodeGenerator : ICodeGenerator<CastEntry>
    {
        private static int CreateKey(VarType from, VarType to) => ((int)from << 8) + (int)to;

        private static Dictionary<int, Action<CodeGeneratorContext>> _codeGeneratorActionsMap = new Dictionary<int, Action<CodeGeneratorContext>>()
        {
            { CreateKey(VarType.Int, VarType.UShort), ToWordUnsigned },
            { CreateKey(VarType.Int, VarType.Short), ToWordSigned },
            { CreateKey(VarType.Int, VarType.Byte), ToByteUnsigned },
            { CreateKey(VarType.Int, VarType.SByte), ToByteSigned },
            { CreateKey(VarType.Int, VarType.Ptr), ToPtr },

            { CreateKey(VarType.UInt, VarType.UShort), ToWordUnsigned },
            { CreateKey(VarType.UInt, VarType.Short), ToWordSigned },
            { CreateKey(VarType.UInt, VarType.Byte), ToByteUnsigned },
            { CreateKey(VarType.UInt, VarType.SByte), ToByteSigned },
            { CreateKey(VarType.UInt, VarType.Ptr), ToPtr },
            { CreateKey(VarType.UInt, VarType.Int), NullConversion },

            { CreateKey(VarType.UShort, VarType.Short), NullConversion },
            { CreateKey(VarType.UShort, VarType.Byte), ToByteUnsigned },
            { CreateKey(VarType.UShort, VarType.SByte), ToByteUnsigned },
            { CreateKey(VarType.UShort, VarType.Ptr), ToPtr },

            { CreateKey(VarType.SByte, VarType.Byte), NullConversion },
            { CreateKey(VarType.SByte, VarType.UShort),NullConversion },

            { CreateKey(VarType.Byte, VarType.Ptr), ToPtr },
            { CreateKey(VarType.Byte, VarType.SByte), NullConversion },
            { CreateKey(VarType.Byte, VarType.Short), NullConversion },

            { CreateKey(VarType.Ptr, VarType.Int), WidenPtr },
            { CreateKey(VarType.Ptr, VarType.UInt), WidenPtr },
            { CreateKey(VarType.Ptr, VarType.UShort), WidenPtr },
            { CreateKey(VarType.Ptr, VarType.Short), WidenPtr },

            { CreateKey(VarType.ByRef, VarType.Ptr), NullConversion },
        };

        public static void NullConversion(CodeGeneratorContext context) 
        { 
            // No code required for this conversion
        }

        public static void ToWordUnsigned(CodeGeneratorContext context)
        {
            context.InstructionsBuilder.Pop(HL);      // LSW
            context.InstructionsBuilder.Pop(DE);      // MSW

            context.InstructionsBuilder.Ld(DE, 0);    // clear msw

            context.InstructionsBuilder.Push(DE);     // MSW
            context.InstructionsBuilder.Push(HL);     // LSW
        }

        public static void ToWordSigned(CodeGeneratorContext context)
        {
            context.InstructionsBuilder.Pop(DE);      // LSW
            context.InstructionsBuilder.Pop(HL);      // MSW

            context.InstructionsBuilder.Ld(H, D);

            context.InstructionsBuilder.Add(HL, HL);  // move sign bit into carry flag
            context.InstructionsBuilder.Sbc(HL, HL);  // hl is now 0 or FFFF

            context.InstructionsBuilder.Push(HL);     // MSW
            context.InstructionsBuilder.Push(DE);     // LSW
        }

        public static void ToByteUnsigned(CodeGeneratorContext context)
        {
            context.InstructionsBuilder.Pop(HL);      // LSW
            context.InstructionsBuilder.Pop(DE);      // MSW

            context.InstructionsBuilder.Ld(DE, 0);    // clear msw
            context.InstructionsBuilder.Ld(H, 0);

            context.InstructionsBuilder.Push(DE);     // MSW
            context.InstructionsBuilder.Push(HL);     // LSW

        }

        public static void ToByteSigned(CodeGeneratorContext context)
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

        public static void ToPtr(CodeGeneratorContext context)
        {
            context.InstructionsBuilder.Pop(HL);      // LSW
            context.InstructionsBuilder.Pop(DE);      // MSW

            context.InstructionsBuilder.Push(HL);     // LSW
        }

        public static void WidenPtr(CodeGeneratorContext context)
        {
            context.InstructionsBuilder.Pop(HL);
            context.InstructionsBuilder.Ld(DE, 0);    // clear msw
            context.InstructionsBuilder.Push(DE);
            context.InstructionsBuilder.Push(HL);
        }

        public void GenerateCode(CastEntry entry, CodeGeneratorContext context)
        {
            var actualType = entry.Op1.Type;
            var desiredType = entry.Type;

            var key = CreateKey(actualType, desiredType);

            // If the types are the same, no cast is needed
            if (actualType == desiredType)
            {
                return;
            }

            if (_codeGeneratorActionsMap.TryGetValue(key, out Action<CodeGeneratorContext>? generateCastCode))
            {
                generateCastCode(context);
            }
            else
            {
                throw new NotImplementedException($"Implicit cast from {actualType} to {desiredType} not supported");
            }
        }
    }
}
