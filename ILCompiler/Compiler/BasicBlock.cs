using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Interfaces;
using ILCompiler.z80;
using System;
using System.Collections.Generic;
using Instruction = ILCompiler.z80.Instruction;

namespace ILCompiler.Compiler
{
    public class BasicBlock
    {
        public BasicBlock Next { get; set; }
        public int StartOffset { get; set; }

        public IList<Instruction> Instructions { get; set; } = new List<Instruction>();
        public EvaluationStack<StackEntry> Stack { get; set; } = new EvaluationStack<StackEntry>(0);

        private bool _marked = false;

        public string Label => $"bb{_id}";
        private readonly int _id;
        private static int nextId = 0;

        private readonly IILImporter _importer;

        public BasicBlock(IILImporter importer, int offset)
        {
            _importer = importer;

            StartOffset = offset;

            _id = nextId++;
        }

        private readonly string[] comparisonRoutinesByOpcode = new string[]
        {
            "EQ",               // Beq
            "GREATERTHANEQ",    // Bge
            "GREATERTHAN",      // Bgt
            "LESSTHANEQ",       // Ble
            "LESSTHAN",         // Blt
            "NOTEQ"             // Bne
        };

        public void ImportBranch(Code opcode, BasicBlock target, BasicBlock fallthrough)
        {
            if (opcode != Code.Br)
            {
                // Gen code here for condition comparison and if true then jump to target basic block via id

                // Possible comparisions are blt, ble, bgt, bge, brfalse, brtrue, beq, bne

                if (opcode == Code.Brfalse || opcode == Code.Brtrue)
                {
                    // Only one argument
                    var op = Stack.Pop();

                    var condition = (opcode == Code.Brfalse) ? Condition.Zero : Condition.NonZero;

                    Append(Instruction.Pop(R16.HL));
                    Append(Instruction.Ld(R16.DE, 0));
                    Append(Instruction.Or(R8.A, R8.A));
                    Append(Instruction.Sbc(R16.HL, R16.DE));
                    Append(Instruction.Jp(condition, target.Label));
                }
                else
                {
                    // two arguments
                    var op1 = Stack.Pop();
                    var op2 = Stack.Pop();

                    Append(Instruction.Pop(R16.HL));
                    Append(Instruction.Pop(R16.DE));

                    Append(Instruction.Call(comparisonRoutinesByOpcode[opcode - Code.Beq]));
                    Append(Instruction.Jp(Condition.C, target.Label));
                }
            }
            else
            {
                Append(Instruction.Jp(target.Label));
            }

            ImportFallThrough(target);

            if (fallthrough != null)
            {
                ImportFallThrough(fallthrough);
            }
        }
        public void ImportFallThrough(BasicBlock next)
        {
            EvaluationStack<StackEntry> entryStack = next.Stack;

            if (entryStack != null)
            {
                // Check the entry stack and the current stack are equivalent,
                // i.e. have same length and elements are identical

                if (entryStack.Length != Stack.Length)
                {
                    throw new InvalidProgramException();
                }

                for (int i = 0; i < entryStack.Length; i++)
                {
                    if (entryStack[i].Kind != Stack[i].Kind)
                    {
                        throw new InvalidProgramException();
                    }

                    // TODO: Should this compare the "Type" of the entries too??
                }
            }
            else
            {
                /*
                if (Stack.Length > 0)
                {
                    entryStack = new EvaluationStack<StackEntry>(Stack.Length);

                    // TODO: Need to understand why this is required
                    for (int i = 0; i < Stack.Length; i++)
                    {
                        entryStack.Push(NewSpillSlot(Stack[i]));
                    }
                }
                next.Stack = entryStack;
                */
            }

            MarkBasicBlock(next);
        }

        private void MarkBasicBlock(BasicBlock basicBlock)
        {
            if (!basicBlock._marked)
            {
                _importer.AddToPendingBasicBlocks(basicBlock);
                basicBlock._marked = true;
            }
        }

        public void ImportBinaryOperation(Code opcode)
        {
            var op1 = Stack.Pop();
            var op2 = Stack.Pop();

            // StackValueKind is carefully ordered to make this work
            StackValueKind kind;
            if (op1.Kind > op2.Kind)
            {
                kind = op1.Kind;
            }
            else
            {
                kind = op2.Kind;
            }

            if (kind != StackValueKind.Int16)
            {
                throw new NotSupportedException("Binary operations on types other than short not supported yet");
            }

            PushExpression(kind);

            Append(Instruction.Pop(R16.DE));
            Append(Instruction.Pop(R16.HL));

            switch (opcode)
            {
                case Code.Add:
                    Append(Instruction.Add(R16.HL, R16.DE));
                    break;

                case Code.Sub:
                    Append(Instruction.Sbc(R16.HL, R16.DE));
                    break;
            }

            Append(Instruction.Push(R16.HL));
        }

        private void PushExpression(StackValueKind kind)
        {
            Stack.Push(new ExpressionEntry(kind));
        }

        public void ImportStoreVar(int index, bool argument)
        {
            var value = Stack.Pop();

            var offset = index * 2; // TODO: This needs to take into account differing sizes of local vars

            Append(Instruction.Pop(R16.HL));
            Append(Instruction.Ld(I16.IX, (short)-(offset + 1), R8.H));
            Append(Instruction.Ld(I16.IX, (short)-(offset + 2), R8.L));
        }

        public void ImportLoadVar(int index, bool argument)
        {
            // TODO: need to actually use proper StackValueKind here!!
            Stack.Push(new ExpressionEntry(StackValueKind.Int16));

            var offset = index * 2; // TODO: This needs to take into account differing sizes of local vars

            Append(Instruction.Ld(R8.H, I16.IX, (short)-(offset + 1)));
            Append(Instruction.Ld(R8.L, I16.IX, (short)-(offset + 2)));
            Append(Instruction.Push(R16.HL));
        }

        public void ImportLdArg(short stackFrameSize)
        {
            // TODO: need to push details of actual arg
            Stack.Push(new ExpressionEntry(StackValueKind.Int16));

            var argumentOffset = stackFrameSize;
            argumentOffset += 2; // accounts for return address
            Append(Instruction.Ld(R8.H, I16.IX, (short)(argumentOffset + 1)));
            Append(Instruction.Ld(R8.L, I16.IX, argumentOffset));
            Append(Instruction.Push(R16.HL));
        }

        public void ImportRet(MethodDef method)
        {
            var hasLocals = method.Body.Variables.Count > 0;
            var hasReturnValue = method.HasReturnType;

            if (hasReturnValue)
            {
                var value = Stack.Pop();
            }

            if (!hasReturnValue && hasLocals)
            {
                Append(Instruction.Ld(R16.SP, I16.IX));
                Append(Instruction.Pop(I16.IX));
            }
            else if (hasReturnValue && !hasLocals)
            {
                Append(Instruction.Pop(R16.BC));
                Append(Instruction.Pop(R16.HL));
                Append(Instruction.Push(R16.BC));
                Append(Instruction.Push(R16.HL));

            }
            else if (hasReturnValue && hasLocals)
            {
                Append(Instruction.Pop(R16.BC));
                Append(Instruction.Ld(R16.SP, I16.IX));
                Append(Instruction.Pop(I16.IX));
                Append(Instruction.Pop(R16.HL));
                Append(Instruction.Push(R16.BC));
                Append(Instruction.Push(R16.HL));
            }

            Append(Instruction.Ret());
        }

        public void ImportCall(MethodDef methodToCall)
        {
            if (methodToCall.DeclaringType.FullName.StartsWith("System.Console"))
            {
                switch (methodToCall.Name)
                {
                    case "Write":
                        var op = Stack.Pop();
                        Append(Instruction.Pop(R16.HL));
                        Append(Instruction.Ld(R8.A, R8.L));
                        Append(Instruction.Call(0x0033)); // ROM routine to display character at current cursor position
                        break;
                }
            }
            else
            {
                var targetMethod = _importer.NameMangler.GetMangledMethodName(methodToCall);
                Append(Instruction.Call(targetMethod));
            }
        }

        public void ImportLoadInt(long value, StackValueKind kind)
        {
            if (kind != StackValueKind.Int16)
            {
                throw new NotSupportedException("Loading anything other than Int16 not currently supported");
            }

            Stack.Push(new Int16ConstantEntry(checked((short)value)));

            Append(Instruction.Ld(R16.HL, (short)value));
            Append(Instruction.Push(R16.HL));
        }


        public void Append(Instruction instruction)
        {
            Instructions.Add(instruction);
        }
    }
}
