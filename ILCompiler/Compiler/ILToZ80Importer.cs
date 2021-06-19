using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.z80;
using System;
using System.Collections.Generic;
using Instruction = ILCompiler.z80.Instruction;

namespace ILCompiler.Compiler
{
    public partial class ILImporter
    {
        private readonly Dictionary<string, string> _labelsToStringData = new Dictionary<string, string>();

        public IList<BasicBlock> Import(Z80MethodCodeNode methodCodeNodeNeedingCode)
        {
            var basicBlockAnalyser = new BasicBlockAnalyser(_method);
            var offsetToIndexMap = basicBlockAnalyser.FindBasicBlocks();
            _basicBlocks = basicBlockAnalyser.BasicBlocks;

            ImportBasicBlocks(offsetToIndexMap);

            var basicBlocks = new List<BasicBlock>();
            for (int i = 0; i < _basicBlocks.Length; i++)
            {
                if (_basicBlocks[i] != null)
                {
                    basicBlocks.Add(_basicBlocks[i]);
                }
            }

            // TODO: This Code gen stuff will be moved out of here soon
#if !NEW_CODEGEN
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

            methodCodeNodeNeedingCode.MethodCode = instructions;
#endif

            return basicBlocks;
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

        private static void GenerateProlog(IList<Instruction> instructions, MethodDef method)
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
            "EQL",              // Beq
            "GREATERTHANEQ",    // Bge
            "GREATERTHAN",      // Bgt
            "LESSTHANEQ",       // Ble
            "LESSTHAN",         // Blt
            "NOTEQ"             // Bne
        };

        private readonly BinaryOp[] binaryOpByOpcode = new BinaryOp[]
        {
            BinaryOp.EQ,        // Beq
            BinaryOp.GE,        // Bge
            BinaryOp.GT,        // Bgt
            BinaryOp.LE,        // Ble
            BinaryOp.LT,        // Blt
            BinaryOp.NE         // Bne
        };

        public void ImportBranch(Code opcode, BasicBlock target, BasicBlock fallthrough)
        {
            // Gen tree generation and type checking
            if (opcode != Code.Br)
            {
                var op1 = _stack.Pop();
                if (op1.Kind != StackValueKind.Int16)
                {
                    throw new NotSupportedException("Boolean comparisonsonly supported using short as underlying type");
                }

                StackEntry op2;
                if (opcode != Code.Brfalse && opcode == Code.Brtrue)
                {
                    op2 = _stack.Pop();
                    if (op2.Kind != StackValueKind.Int16)
                    {
                        throw new NotSupportedException("Boolean comparisonsonly supported using short as underlying type");
                    }
                }
                else
                {
                    op2 = new Int16ConstantEntry((short)(opcode == Code.Brfalse ? 0 : 1));
                }
                op1 = new BinaryOperator(BinaryOp.EQ, op1, op2, StackValueKind.Int16);
                ImportAppendTree(new JumpTrueEntry(op1));
            }
            else
            {
                // Don't generate any gentree for a branch.
            }

            // Code gen
            if (opcode != Code.Br)
            {
                if (opcode == Code.Brfalse || opcode == Code.Brtrue)
                {
                    var condition = (opcode == Code.Brfalse) ? Condition.Zero : Condition.NonZero;
                    Append(Instruction.Pop(R16.HL));
                    Append(Instruction.Ld(R16.DE, 0));
                    Append(Instruction.Or(R8.A, R8.A));
                    Append(Instruction.Sbc(R16.HL, R16.DE));
                    Append(Instruction.Jp(condition, target.Label));
                }
                else
                {
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

            // Fall through handling
            ImportFallThrough(target);

            if (fallthrough != null)
            {
                ImportFallThrough(fallthrough);
            }
        }

        public void ImportFallThrough(BasicBlock next)
        {
            // Evaluation stack in each basic block holds the imported high level tree representation of the IL

            /*
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
            // Gen tree generation and type checking
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

            BinaryOp binaryOp = opcode == Code.Add ? BinaryOp.ADD : (opcode == Code.Sub ? BinaryOp.SUB : BinaryOp.MUL);           
            var binaryExpr = new BinaryOperator(binaryOp, op1, op2, kind);
            PushExpression(binaryExpr);

            // Code gen
#if !NEW_CODEGEN
            switch (opcode)
            {
                case Code.Add:
                    Append(Instruction.Pop(R16.DE));
                    Append(Instruction.Pop(R16.HL));
                    Append(Instruction.Add(R16.HL, R16.DE));
                    break;

                case Code.Sub:
                    Append(Instruction.Pop(R16.DE));
                    Append(Instruction.Pop(R16.HL));
                    Append(Instruction.Sbc(R16.HL, R16.DE));
                    break;

                case Code.Mul:
                    Append(Instruction.Pop(R16.DE));
                    Append(Instruction.Pop(R16.BC));
                    Append(Instruction.Call("MUL16"));
                    break;
            }
            Append(Instruction.Push(R16.HL));
#endif
        }

        public void ImportStoreIndirect(WellKnownType type)
        {
            // Gen tree generation and type checking
            var value = _stack.Pop();
            var addr = _stack.Pop();

            if (type != WellKnownType.SByte || addr.Kind != StackValueKind.Int16 || value.Kind != StackValueKind.Int16)
            {
                throw new NotSupportedException();
            }

            var addrNode = new IndEntry(addr);
            ImportAppendTree(new AssignmentEntry(addrNode, value));

            // Code gen
            Append(Instruction.Pop(R16.BC));
            Append(Instruction.Pop(R16.HL));
            Append(Instruction.LdInd(R16.HL, R8.C));
        }

        public void ImportStoreVar(int index, bool argument)
        {
            // Gen tree generation and type checking
            var value = _stack.Pop();
            if (value.Kind != StackValueKind.Int16 && value.Kind != StackValueKind.ObjRef)
            {
                throw new NotSupportedException("Storing variables other than short or object refs not supported yet");
            }
            var op2 = new LocalVariableEntry(index, value.Kind);
            var assignNode = new AssignmentEntry(op2, value);
            ImportAppendTree(assignNode);

            // Code gen
            var offset = index * 2; // TODO: This needs to take into account differing sizes of local vars

            Append(Instruction.Pop(R16.HL));
            Append(Instruction.Ld(I16.IX, (short)-(offset + 1), R8.H));
            Append(Instruction.Ld(I16.IX, (short)-(offset + 2), R8.L));
        }

        public void ImportLoadVar(int index, bool argument)
        {
            // Gen tree generation and type checking
            var localNumber = _method.Parameters.Count + index;
            var node = new LocalVariableEntry(localNumber, StackValueKind.Int16);
            PushExpression(node);

            // Code gen
#if !NEW_CODEGEN
            var offset = index * 2; // TODO: This needs to take into account differing sizes of local vars

            Append(Instruction.Ld(R8.H, I16.IX, (short)-(offset + 1)));
            Append(Instruction.Ld(R8.L, I16.IX, (short)-(offset + 2)));
            Append(Instruction.Push(R16.HL));
#endif
        }

        public void ImportLdArg(int index)
        {
            // Gen tree generation and type checking

            var node = new LocalVariableEntry(index, StackValueKind.Int16);
            PushExpression(node);

            // Code gen
            var offset = index * 2; // TODO: This needs to take into account differing sizes of parameters

            Append(Instruction.Ld(R8.H, I16.IY, (short)-(offset + 1)));
            Append(Instruction.Ld(R8.L, I16.IY, (short)-(offset + 2)));
            Append(Instruction.Push(R16.HL));
        }

        public void ImportLoadInt(long value, StackValueKind kind)
        {
            // Gen tree generation and type checking
            if (kind == StackValueKind.Int16)
            {
                PushExpression(new Int16ConstantEntry(checked((short)value)));
            }
            else if (kind == StackValueKind.Int32)
            {
                PushExpression(new Int32ConstantEntry(checked((int)value)));
            }
            else
            {
                throw new NotSupportedException("Loading anything other than Int16 not currently supported");
            }

            // Code gen
#if !NEW_CODEGEN
            if (kind == StackValueKind.Int16)
            {
                Append(Instruction.Ld(R16.HL, (short)value));
                Append(Instruction.Push(R16.HL));
            }
            else if (kind == StackValueKind.Int32)
            {
                var low = BitConverter.ToInt16(BitConverter.GetBytes(value), 0);
                var high = BitConverter.ToInt16(BitConverter.GetBytes(value), 2);

                Append(Instruction.Ld(R16.HL, low));
                Append(Instruction.Push(R16.HL));
                Append(Instruction.Ld(R16.HL, high));
                Append(Instruction.Push(R16.HL));
            }
#endif
        }

        public void ImportLoadString(string str)
        {
            // Gen tree generation and type checking
            PushExpression(new StringConstantEntry(str));

            // Code generation
            var label = LabelGenerator.GetLabel(LabelType.String);
            _labelsToStringData[label] = str;

            Append(Instruction.Ld(R16.HL, label));
            Append(Instruction.Push(R16.HL));
        }

        private bool ImportIntrinsicCall(MethodDef methodToCall)
        {
            // Gen tree generation and type checking
            IList<StackEntry> arguments = new List<StackEntry>();
            for (int i = 0; i < methodToCall.Parameters.Count; i++)
            {
                var argument = _stack.Pop();
                arguments.Add(argument);
            }

            // Not yet implemented methods with non void return type
            if (methodToCall.HasReturnType)
            {
                throw new NotSupportedException();
            }

            var targetMethod = _compilation.NameMangler.GetMangledMethodName(methodToCall);
            var callNode = new IntrinsicEntry(targetMethod, arguments, StackValueKind.Unknown);
            ImportAppendTree(callNode);

            // Code gen
            switch (methodToCall.Name)
            {
                // TODO: Suspect this won't stay as an intrinsic but at least we have the mechanism for instrincs
                case "Write":
                    if (IsTypeName(methodToCall, "System", "Console"))
                    {
                        var argtype = methodToCall.Parameters[0].Type;
                        if (argtype.FullName == "System.String")
                        {
                            Append(Instruction.Pop(R16.HL));    // put argument 1 into HL
                            Append(Instruction.Call("PRINT"));
                        }
                        else if (argtype.FullName == "System.Int16")
                        {
                            Append(Instruction.Pop(R16.HL));    // put argument 1 into HL
                            Append(Instruction.Call("NUM2DEC"));
                        }
                        else
                        {
                            // TODO: Need to fix this to display number properly
                            // rather than take low 8 bits and treat as ascii
                            Append(Instruction.Pop(R16.HL));    // put argument 1 into HL
                            Append(Instruction.Ld(R8.A, R8.L)); // Load low byte of argument 1 into A
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
            // Intrinsic calls
            if (methodToCall.IsIntrinsic())
            {
                if (!ImportIntrinsicCall(methodToCall))
                {
                    throw new NotSupportedException("Unknown intrinsic");
                }
                return;
            }

            // Pinvoke calls
            if (methodToCall.IsPinvokeImpl)
            {
                Append(Instruction.Call(methodToCall.ImplMap.Name));
                return;
            }

            // Gen tree generation and type checking
            IList<StackEntry> arguments = new List<StackEntry>();
            for (int i = 0; i < methodToCall.Parameters.Count; i++)
            {
                var argument = _stack.Pop();
                arguments.Add(argument);
            }

            // Not yet implemented methods with non void return type
            if (methodToCall.HasReturnType)
            {
                throw new NotSupportedException();
            }

            var targetMethod = _compilation.NameMangler.GetMangledMethodName(methodToCall);
            var callNode = new CallEntry(targetMethod, arguments, StackValueKind.Unknown);
            ImportAppendTree(callNode);

            // Code gen
#if !NEW_CODEGEN
            Append(Instruction.Call(targetMethod));
#endif
        }

        public void ImportRet(MethodDef method)
        {
            // Gen tree generation and type checking
            var hasReturnValue = method.HasReturnType;
            var retNode = new ReturnEntry();
            if (hasReturnValue)
            {
                var value = _stack.Pop();
                if (value.Kind != StackValueKind.Int16)
                {
                    throw new NotSupportedException("Return values of types other than short not supported yet");
                }
                retNode.Return = value;
            }
            ImportAppendTree(retNode);

            // Code gen
#if !NEW_CODEGEN
            var hasParameters = method.Parameters.Count > 0;
            var hasLocals = method.Body.Variables.Count > 0;

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
                Append(Instruction.Pop(R16.BC));            // Remove IY
                Append(Instruction.Pop(R16.HL));            // Store return address in HL
                Append(Instruction.Ld(R16.SP, I16.IY));     // Reset SP to before arguments

                Append(Instruction.Push(R16.BC));           // Restore IY
                Append(Instruction.Pop(I16.IY)); 
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
#endif
        }

        public void Append(Instruction instruction)
        {
            _currentBasicBlock.Instructions.Add(instruction);
        }
    }
}
