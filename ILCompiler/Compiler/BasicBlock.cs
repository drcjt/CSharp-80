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

        public EvaluationStack<StackEntry> EntryStack { get; set; } = new EvaluationStack<StackEntry>(0);

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

        public void ImportBranch(Code opcode, BasicBlock target, BasicBlock fallthrough)
        {
            if (opcode != Code.Br || opcode != Code.Br_S)
            {
                // Gen code here for condition comparison and if true then jump to target basic block via id

                // Possible comparisions are blt, ble, bgt, bge, brfalse, brtrue, beq, bne

                if (opcode == Code.Brfalse || opcode == Code.Brtrue || opcode == Code.Brfalse_S || opcode == Code.Brtrue_S)
                {
                    // Only one argument
                    var op = EntryStack.Pop();

                    var condition = (opcode == Code.Brfalse || opcode == Code.Brfalse_S) ? Condition.Zero : Condition.NonZero;

                    Append(Instruction.Pop(R16.HL));
                    Append(Instruction.Ld(R16.DE, 0));
                    Append(Instruction.Or(R8.A, R8.A));
                    Append(Instruction.Sbc(R16.HL, R16.DE));
                    Append(Instruction.Jp(condition, target.Label));
                }
                else
                {
                    // two arguments

                    // pop into hl
                    // pop into de
                    // clear a
                    // sbc hl, de
                    // jp z or nz
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
            EvaluationStack<StackEntry> entryStack = next.EntryStack;

            /*
             * Temporarily commenting out till more instructions implemented as
             * causing issues in interim
            if (entryStack != null)
            {
                // Check the entry stack and the current stack are equivalent,
                // i.e. have same length and elements are identical

                if (entryStack.Length != _stack.Length)
                {
                    throw new InvalidProgramException();
                }

                for (int i = 0; i < entryStack.Length; i++)
                {
                    if (entryStack[i].Kind != _stack[i].Kind)
                    {
                        throw new InvalidProgramException();
                    }

                    // TODO: Should this compare the "Type" of the entries too??
                }
            }
            else
            {
                if (_stack.Length > 0)
                {
                    entryStack = new EvaluationStack<StackEntry>(_stack.Length);

                    // TODO: Need to understand why this is required
                    for (int i = 0; i < _stack.Length; i++)
                    {
                        entryStack.Push(NewSpillSlot(_stack[i]));
                    }
                }
                next.EntryStack = entryStack;
            }
            */

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
            var op1 = EntryStack.Pop();
            var op2 = EntryStack.Pop();

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

            Append(Instruction.Pop(R16.HL));
            Append(Instruction.Pop(R16.DE));

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
            EntryStack.Push(new ExpressionEntry(kind));
        }

        public void ImportLdArg(short stackFrameSize)
        {
            var argumentOffset = stackFrameSize;
            argumentOffset += 2; // accounts for return address
            Append(Instruction.Ld(R8.H, I16.IX, (short)(argumentOffset + 1)));
            Append(Instruction.Ld(R8.L, I16.IX, argumentOffset));
            Append(Instruction.Push(R16.HL));
        }

        public void ImportRet(MethodDef method)
        {
            if (method.ReturnType.TypeName != "Void")
            {
                Append(Instruction.Pop(R16.BC));
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
                        Append(Instruction.Pop(R16.HL));
                        Append(Instruction.Ld(R8.A, R8.L));
                        Append(Instruction.Call(0x0033)); // ROM routine to display character at current cursor position
                        break;
                }
            }
            else
            {
                var targetMethod = methodToCall.Name;
                Append(Instruction.Call(targetMethod));
            }
        }

        public void ImportLoadInt(long value, StackValueKind kind)
        {
            if (kind != StackValueKind.Int16)
            {
                throw new NotSupportedException("Loading anything other than Int16 not currently supported");
            }

            EntryStack.Push(new Int16ConstantEntry(checked((short)value)));

            Append(Instruction.Ld(R16.HL, (short)value));
            Append(Instruction.Push(R16.HL));
        }


        public void Append(Instruction instruction)
        {
            Instructions.Add(instruction);
        }
    }
}
