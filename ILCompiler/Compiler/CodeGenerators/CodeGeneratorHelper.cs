using ILCompiler.Compiler.Emit;
using static ILCompiler.Compiler.Emit.Registers;

namespace ILCompiler.Compiler.CodeGenerators
{
    internal static class CodeGeneratorHelper
    {
        public static void AddHLFromDE(InstructionsBuilder builder, short value)
        {
            if (value != 0)
            {
                if (-3 <= value && value <= 3)
                {
                    // For values upto +/- 3 we can just use Inc/Dec
                    // These instructions use 1 byte, 6 t-states                
                    IncOrDecNTimes(builder, value > 0, Math.Abs(value), HL);
                }
                else
                {
                    builder.Ld(DE, (short)value);       // 3 bytes, 10 t-states
                    builder.Add(HL, DE);                // 1 byte, 11 t-states
                }
            }
        }

        public static void AddSPFromHL(InstructionsBuilder builder, short value)
        {
            if (value != 0) 
            {
                if (-5 <= value && value <= 5)
                {
                    if (value % 2 == 0 && value > 0)
                    {
                        // Can use pop to alter sp
                        for (int i = 0; i < value / 2; i++)
                        {
                            builder.Pop(AF);
                        }
                    }
                    else
                    {
                        // For values upto +/- 3 we can just use Inc/Dec
                        // These instructions use 1 byte, 6 t-states
                        IncOrDecNTimes(builder, value > 0, Math.Abs(value), SP);
                    }
                }
                else
                {
                    // 5 bytes, 10 + 11 + 6 = 27 t-states
                    builder.Ld(HL, (short)value);
                    builder.Add(HL, SP);
                    builder.Ld(SP, HL);
                }
            }
        }

        public static void IncOrDecNTimes(InstructionsBuilder builder, bool inc, int count, Register16 register)
        {
            for (int i = 0; i < count; i++)
            {
                if (inc)
                {
                    builder.Inc(register);
                }
                else
                {
                    builder.Dec(register);
                }
            }
        }
    }
}
