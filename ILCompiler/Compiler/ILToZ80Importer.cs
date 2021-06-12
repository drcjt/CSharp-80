using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.z80;
using System;
using Instruction = ILCompiler.z80.Instruction;

namespace ILCompiler.Compiler
{
    public partial class ILImporter
    {
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
                    var op = _stack.Pop();
                    if (op.Kind != StackValueKind.Int16)
                    {
                        throw new NotSupportedException("Boolean comparisonsonly supported using short as underlying type");
                    }

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
                    var op1 = _stack.Pop();
                    var op2 = _stack.Pop();

                    if (op1.Kind != StackValueKind.Int16 && op2.Kind != StackValueKind.Int16)
                    {
                        throw new NotSupportedException("Binary operations on types other than short not supported yet");
                    }

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
            /*
             * TODO: Work out why we need to record an evaluation stack against each basic block
             * 
            EvaluationStack<StackEntry> entryStack = next.Stack;

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
                
            }
            */
            MarkBasicBlock(next);
        }

        public void ImportBinaryOperation(Code opcode)
        {
            var op1 = _stack.Pop();
            var op2 = _stack.Pop();

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

        public void ImportStoreVar(int index, bool argument)
        {
            var value = _stack.Pop();
            if (value.Kind != StackValueKind.Int16)
            {
                throw new NotSupportedException("Storing variables other than short not supported yet");
            }

            var offset = index * 2; // TODO: This needs to take into account differing sizes of local vars

            Append(Instruction.Pop(R16.HL));
            Append(Instruction.Ld(I16.IX, (short)-(offset + 1), R8.H));
            Append(Instruction.Ld(I16.IX, (short)-(offset + 2), R8.L));
        }

        public void ImportLoadVar(int index, bool argument)
        {
            // TODO: need to actually use proper StackValueKind here!!
            _stack.Push(new ExpressionEntry(StackValueKind.Int16));

            var offset = index * 2; // TODO: This needs to take into account differing sizes of local vars

            Append(Instruction.Ld(R8.H, I16.IX, (short)-(offset + 1)));
            Append(Instruction.Ld(R8.L, I16.IX, (short)-(offset + 2)));
            Append(Instruction.Push(R16.HL));
        }

        public void ImportLdArg(short stackFrameSize)
        {
            // TODO: need to push details of actual arg
            _stack.Push(new ExpressionEntry(StackValueKind.Int16));

            var argumentOffset = stackFrameSize;
            argumentOffset += 2; // accounts for return address
            Append(Instruction.Ld(R8.H, I16.IX, (short)(argumentOffset + 1)));
            Append(Instruction.Ld(R8.L, I16.IX, argumentOffset));
            Append(Instruction.Push(R16.HL));
        }

        public void ImportLoadInt(long value, StackValueKind kind)
        {
            if (kind != StackValueKind.Int16)
            {
                throw new NotSupportedException("Loading anything other than Int16 not currently supported");
            }

            _stack.Push(new Int16ConstantEntry(checked((short)value)));

            Append(Instruction.Ld(R16.HL, (short)value));
            Append(Instruction.Push(R16.HL));
        }

        public void ImportCall(MethodDef methodToCall)
        {
            if (methodToCall.DeclaringType.FullName.StartsWith("System.Console"))
            {
                switch (methodToCall.Name)
                {
                    case "Write":
                        var op = _stack.Pop();
                        Append(Instruction.Pop(R16.HL));
                        Append(Instruction.Ld(R8.A, R8.L));
                        Append(Instruction.Call(0x0033)); // ROM routine to display character at current cursor position
                        break;
                }
            }
            else
            {
                var targetMethod = _compilation.NameMangler.GetMangledMethodName(methodToCall);
                Append(Instruction.Call(targetMethod));
            }
        }

        public void ImportRet(MethodDef method)
        {
            var hasLocals = method.Body.Variables.Count > 0;
            var hasReturnValue = method.HasReturnType;

            if (hasReturnValue)
            {
                var value = _stack.Pop();
                if (value.Kind != StackValueKind.Int16)
                {
                    throw new NotSupportedException("Return values of types other than short not supported yet");
                }
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
    }
}
