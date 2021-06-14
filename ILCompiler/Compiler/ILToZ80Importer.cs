using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.z80;
using System;
using System.Collections.Generic;
using Instruction = ILCompiler.z80.Instruction;

namespace ILCompiler.Compiler
{
    public partial class ILImporter
    {
        private Dictionary<string, string> _labelsToStringData = new Dictionary<string, string>();

        public void Compile(Z80MethodCodeNode methodCodeNodeNeedingCode)
        {
            var basicBlockAnalyser = new BasicBlockAnalyser(_method);
            var offsetToIndexMap = basicBlockAnalyser.FindBasicBlocks();
            _basicBlocks = basicBlockAnalyser.BasicBlocks;

            ImportBasicBlocks(offsetToIndexMap);  // This converts IL to Z80

            List<Instruction> instructions = new();
            GenerateStringData(instructions);

            instructions.Add(new LabelInstruction(_compilation.NameMangler.GetMangledMethodName(methodCodeNodeNeedingCode.Method)));

            GenerateProlog(instructions, methodCodeNodeNeedingCode.Method);

            // Loop thru basic blocks here to generate overall code for whole method
            for (int i = 0; i < _basicBlocks.Length; i++)
            {
                var basicBlock = _basicBlocks[i];
                if (basicBlock != null)
                {
                    // Run optimization phases on basic blocks here
                    _compilation.Optimizer.Optimize(basicBlock.Instructions);

                    instructions.Add(new LabelInstruction(basicBlock.Label));
                    instructions.AddRange(basicBlock.Instructions);
                }
            }

            methodCodeNodeNeedingCode.SetCode(instructions);
        }

        private void GenerateStringData(IList<Instruction> instructions)
        {
            // TODO: Need to eliminate duplicate strings

            foreach (var keyValuePair in _labelsToStringData)
            {
                instructions.Add(new LabelInstruction(keyValuePair.Key));
                foreach (var ch in keyValuePair.Value)
                {
                    instructions.Add(Instruction.Db((byte)ch));
                }
                instructions.Add(Instruction.Db(0));
            }
        }

        private void GenerateProlog(IList<Instruction> instructions, MethodDef method)
        {
            // TODO: This assumes all locals are 16 bit in size

            var paramsCount = method.Parameters.Count;
            if (paramsCount > 0)
            {
                instructions.Add(Instruction.Push(I16.IY));
                // Set IY to start of arguments here
                // IY = SP - (2 * (number of params + 2))

                instructions.Add(Instruction.Ld(R16.HL, (short)(2 * (paramsCount + 2))));
                instructions.Add(Instruction.Add(R16.HL, R16.SP));
                instructions.Add(Instruction.Push(R16.HL));
                instructions.Add(Instruction.Pop(I16.IY));
            }

            var localsCount = method.Body.Variables.Count;
            if (localsCount > 0)
            {
                instructions.Add(Instruction.Push(I16.IX));
                instructions.Add(Instruction.Ld(I16.IX, 0));
                instructions.Add(Instruction.Add(I16.IX, R16.SP));

                var localsSize = localsCount * 2;

                instructions.Add(Instruction.Ld(R16.HL, (short)-localsSize));
                instructions.Add(Instruction.Add(R16.HL, R16.SP));
                instructions.Add(Instruction.Ld(R16.SP, R16.HL));
            }
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

        public void ImportStoreIndirect(WellKnownType type)
        {
            var value = _stack.Pop();
            var addr = _stack.Pop();

            if (type == WellKnownType.SByte && addr.Kind == StackValueKind.Int16 && value.Kind == StackValueKind.Int16)
            {
                Append(Instruction.Pop(R16.BC));
                Append(Instruction.Pop(R16.HL));

                Append(Instruction.LdInd(R16.HL, R8.C));
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public void ImportStoreVar(int index, bool argument)
        {
            var value = _stack.Pop();
            if (value.Kind != StackValueKind.Int16 && value.Kind != StackValueKind.ObjRef)
            {
                throw new NotSupportedException("Storing variables other than short or object refs not supported yet");
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

        public void ImportLdArg(int index)
        {
            // TODO: need to push details of actual arg
            _stack.Push(new ExpressionEntry(StackValueKind.Int16));

            var offset = index * 2; // TODO: This needs to take into account differing sizes of parameters

            Append(Instruction.Ld(R8.H, I16.IY, (short)-(offset + 1)));
            Append(Instruction.Ld(R8.L, I16.IY, (short)-(offset + 2)));
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

        public void ImportLoadString(string str)
        {
            _stack.Push(new ExpressionEntry(StackValueKind.ObjRef));

            var label = LabelGenerator.GetLabel(LabelType.String);
            _labelsToStringData[label] = str;

            Append(Instruction.Ld(R16.HL, label));
            Append(Instruction.Push(R16.HL));
        }

        private bool ImportIntrinsicCall(MethodDef methodToCall)
        {

            switch (methodToCall.Name)
            {
                // TODO: Suspect this won't stay as an intrinsic but at least we have the mechanism for instrincs
                case "Write":
                    if (IsTypeName(methodToCall, "System", "Console"))
                    {
                        var argtype = methodToCall.Parameters[0].Type;
                        if (argtype.FullName == "System.String")
                        {
                            Append(Instruction.Pop(R16.HL));
                            Append(Instruction.Call("PRINT"));
                        }
                        else
                        {
                            Append(Instruction.Pop(R16.HL));
                            Append(Instruction.Ld(R8.A, R8.L));
                            Append(Instruction.Call(0x0033)); // ROM routine to display character at current cursor position
                        }

                        return true;
                    }
                    break;
                default:
                    break;
            }

            return false;
        }

        private static bool IsTypeName(MethodDef method, string typeNamespace, string typeName)
        {
            var metadataType = method.DeclaringType;
            if (metadataType == null)
            {
                return false;
            }
            return metadataType.Namespace == typeNamespace && metadataType.Name == typeName;
        }

        public void ImportCall(MethodDef methodToCall)
        {
            if (methodToCall.IsIntrinsic())
            {
                if (!ImportIntrinsicCall(methodToCall))
                {
                    throw new NotSupportedException("Unknown intrinsic");
                }
                return;
            }

            if (methodToCall.IsPinvokeImpl)
            {
                Append(Instruction.Call(methodToCall.ImplMap.Name));
                return;
            }

            var targetMethod = _compilation.NameMangler.GetMangledMethodName(methodToCall);
            Append(Instruction.Call(targetMethod));
        }

        public void ImportRet(MethodDef method)
        {
            var hasParameters = method.Parameters.Count > 0;
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

            if (hasReturnValue)
            {
                Append(Instruction.Pop(R16.BC));            // Copy return value into BC
            }

            if (hasLocals)
            {
                Append(Instruction.Ld(R16.SP, I16.IX));     // Move SP to before locals
                Append(Instruction.Pop(I16.IX));            // Remove IX
            }

            if (hasParameters)
            {
                Append(Instruction.Pop(R16.HL));            // Remove IY
                Append(Instruction.Pop(R16.HL));            // Store return address in HL
                Append(Instruction.Ld(R16.SP, I16.IY));     // Reset SP to before arguments
            }

            if (hasReturnValue)
            {
                Append(Instruction.Pop(R16.HL));            // Store return address in HL
                Append(Instruction.Push(R16.BC));           // Push return value
                Append(Instruction.Push(R16.HL));           // Push return address
            }
            else if (hasParameters)
            {
                Append(Instruction.Push(R16.HL));           // Push return address
            }

            Append(Instruction.Ret());
        }

        public void Append(Instruction instruction)
        {
            _currentBasicBlock.Instructions.Add(instruction);
        }
    }
}
