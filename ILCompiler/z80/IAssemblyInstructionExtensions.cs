using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILCompiler.z80
{
    public static class IAssemblyInstructionExtensions
    {
        public static void Ret(this IAssembly assembly)
        {
            assembly.Add(new Instruction(Opcode.Ret, string.Empty));
        }
        public static void Pop(this IAssembly assembly, Register target)
        {
            var lastInstruction = assembly.Last;
            if (lastInstruction.Opcode == Opcode.Push && lastInstruction.Operands == target.ToString())
            {
                // Eliminate PUSH XX followed by POP XX
                assembly.RemoveLast();
            }
            else
            {
                assembly.Add(new Instruction(Opcode.Pop, target.ToString()));
            }
        }

        public static void Push(this IAssembly assembly, Register target)
        {
            assembly.Add(new Instruction(Opcode.Push, target.ToString()));
        }

        public static void Ld(this IAssembly assembly, Register target, short source)
        {
            assembly.Add(new Instruction(Opcode.Ld, target.ToString() + ", " + string.Format("{0:X}H", source)));
        }

        public static void Ld(this IAssembly assembly, R8Type target, I16Type source, short offset)
        {
            assembly.Add(new Instruction(Opcode.Ld, target.ToString() + ", (" + source.ToString() + string.Format("{0:+#;-#;+0}", offset) + ")"));
        }

        public static void Ld(this IAssembly assembly, R16Type target, R16Type source)
        {
            assembly.Add(new Instruction(Opcode.Ld, target.ToString() + ", " + source.ToString()));
        }

        public static void Ld(this IAssembly assembly, R8Type target, R8Type source)
        {
            assembly.Add(new Instruction(Opcode.Ld, target.ToString() + ", " + source.ToString()));
        }

        public static void Call(this IAssembly assembly, string label)
        {
            assembly.Add(new Instruction(Opcode.Call, label));
        }

        public static void Call(this IAssembly assembly, UInt16 target)
        {
            assembly.Add(new Instruction(Opcode.Call, string.Format("{0:X}H", target)));
        }

        public static void Add(this IAssembly assembly, Register target, Register source)
        {
            assembly.Add(new Instruction(Opcode.Add, target.ToString() + ", " + source.ToString()));
        }

        public static void Ex(this IAssembly assembly, Register target, Register source)
        {
            assembly.Add(new Instruction(Opcode.Ex, target.ToString() + ", " + source.ToString()));
        }
    }
}
