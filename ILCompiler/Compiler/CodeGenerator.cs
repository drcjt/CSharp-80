using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.EvaluationStack;
using Z80Assembler;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler
{
    // TODO: This shouldn't really have any dependencies on dnlib/IL stuff
    public class CodeGenerator
    {
        private readonly Compilation _compilation;
        private readonly IList<LocalVariableDescriptor> _localVariableTable;
        private readonly Z80MethodCodeNode _methodCodeNode;

        private readonly Dictionary<string, string> _labelsToStringData = new Dictionary<string, string>();

        private readonly Assembler _currentAssembler = new Assembler();

        public CodeGenerator(Compilation compilation, IList<LocalVariableDescriptor> localVariableTable, Z80MethodCodeNode methodCodeNodeNeedingCode)
        {
            _compilation = compilation;
            _localVariableTable = localVariableTable;
            _methodCodeNode = methodCodeNodeNeedingCode;
        }

        public IList<Instruction> Generate(IList<BasicBlock> blocks)
        {
            var methodInstructions = new List<Instruction>();

            GenerateStringMap(blocks);
            GenerateStringData(_currentAssembler);

            _currentAssembler.AddInstruction(new LabelInstruction(_compilation.NameMangler.GetMangledMethodName(_methodCodeNode.Method)));

            GenerateProlog(_currentAssembler);
            methodInstructions.AddRange(_currentAssembler.Instructions);

            foreach (var block in blocks)
            {
                _currentAssembler.Reset();

                _currentAssembler.AddInstruction(new LabelInstruction(block.Label));

                var currentNode = block.FirstNode;
                while (currentNode != null)
                {
                    GenerateFromNode(currentNode);
                    currentNode = currentNode.Next;
                }

                Optimize(_currentAssembler.Instructions);
                methodInstructions.AddRange(_currentAssembler.Instructions);
            }

            return methodInstructions;
        }

        private void GenerateFromNode(StackEntry node)
        {
            switch (node.Operation)
            {
                case Operation.StoreIndirect:
                    GenerateCodeForStoreIndirect(node as StoreIndEntry);
                    break;

                case Operation.Return:
                    GenerateCodeForReturn(node as ReturnEntry);
                    break;

                case Operation.Constant_Int32:
                    GenerateCodeForInt32Constant(node as Int32ConstantEntry);
                    break;

                case Operation.Constant_String:
                    GenerateCodeForStringConstant(node as StringConstantEntry);
                    break;

                case Operation.JumpTrue:
                    GenerateCodeForJumpTrue(node as JumpTrueEntry);
                    break;

                case Operation.Jump:
                    GenerateCodeForJump(node as JumpEntry);
                    break;

                case Operation.Neg:
                    GenerateCodeForNeg(node as UnaryOperator);
                    break;

                case Operation.Eq:
                case Operation.Ne:
                case Operation.Lt:
                case Operation.Le:
                case Operation.Gt:
                case Operation.Ge:
                    GenerateCodeForComparison(node as BinaryOperator);
                    break;

                case Operation.Add:
                case Operation.Mul:
                case Operation.Sub:
                case Operation.Div:
                case Operation.Rem:
                case Operation.Div_Un:
                case Operation.Rem_Un:
                    GenerateCodeForBinaryOperator(node as BinaryOperator);
                    break;

                case Operation.LocalVariable:
                    GenerateCodeForLocalVariable(node as LocalVariableEntry);
                    break;

                case Operation.StoreLocalVariable:
                    GenerateCodeForStoreLocalVariable(node as StoreLocalVariableEntry);
                    break;

                case Operation.Call:
                    GenerateCodeForCall(node as CallEntry);
                    break;

                case Operation.Intrinsic:
                    GenerateCodeForIntrinsic(node as IntrinsicEntry);
                    break;

                case Operation.Cast:
                    GenerateCodeForCast(node as CastEntry);
                    break;

                default:
                    throw new NotImplementedException($"Unimplemented node type {node.Operation}");
            }
        }

        // TODO: Consider making this a separate phase
        private void GenerateStringMap(IList<BasicBlock> blocks)
        {
            // Process all stack entrys and extract string definitions to populate the string map
            foreach (var block in blocks)
            {
                var currentNode = block.FirstNode;
                while (currentNode != null)
                {
                    if (currentNode is StringConstantEntry)
                    {
                        var stringConstantEntry = currentNode as StringConstantEntry;

                        var label = LabelGenerator.GetLabel(LabelType.String);
                        _labelsToStringData[label] = stringConstantEntry.Value;

                        stringConstantEntry.Label = label;
                    }
                    currentNode = currentNode.Next;
                }
            }
        }

        private void GenerateStringData(Assembler assembler)
        {
            // TODO: Need to eliminate duplicate strings
            foreach (var keyValuePair in _labelsToStringData)
            {
                assembler.AddInstruction(new LabelInstruction(keyValuePair.Key));
                foreach (var ch in keyValuePair.Value)
                {
                    assembler.Db((byte)ch);
                }
                assembler.Db(0);
            }
        }
        private void GenerateProlog(Assembler assembler)
        {
            // Stack frame looks like this:
            //
            //     |                       |
            //     |-----------------------|   <-- IY will point to here when method code executes
            //     |       incoming        |
            //     |       arguments       |
            //     |-----------------------|
            //     |    return address     |
            //     +=======================+
            //     |    IY (if arguments)  |
            //     |     IX (if locals)    |
            //     |-----------------------|   <-- IX will point to here when method code executes
            //     |    Local variables    |
            //     |-----------------------|
            //     |   Arguments for the   |
            //     ~     next method       ~
            //     |                       |
            //     |      |                |
            //     |      | Stack grows    |
            //            | downward
            //            V

            var parametersSize = 0;
            var localsSize = 0;
            foreach (var localVariable in _localVariableTable)
            {
                if (localVariable.IsParameter)
                {
                    parametersSize += localVariable.ExactSize;
                }
                else
                {
                    localsSize += localVariable.ExactSize;
                }
            }

            if (_methodCodeNode.ParamsCount > 0)
            {
                assembler.Push(I16.IY);
                // Set IY to start of arguments here
                // IY = SP - 4 - size of parameters

                assembler.Ld(R16.HL, (short)(4 + parametersSize));
                assembler.Add(R16.HL, R16.SP);
                assembler.Push(R16.HL);
                assembler.Pop(I16.IY);
            }

            if (_methodCodeNode.LocalsCount > 0)
            {
                assembler.Push(I16.IX);
                assembler.Ld(I16.IX, 0);
                assembler.Add(I16.IX, R16.SP);

                assembler.Ld(R16.HL, (short)-localsSize);
                assembler.Add(R16.HL, R16.SP);
                assembler.Ld(R16.SP, R16.HL);
            }
        }

        public void GenerateCodeForInt32Constant(Int32ConstantEntry entry)
        {
            var value = (entry as Int32ConstantEntry).Value;
            var low = BitConverter.ToInt16(BitConverter.GetBytes(value), 0);
            var high = BitConverter.ToInt16(BitConverter.GetBytes(value), 2);

            _currentAssembler.Ld(R16.HL, low);
            _currentAssembler.Push(R16.HL);
            _currentAssembler.Ld(R16.HL, high);
            _currentAssembler.Push(R16.HL);
        }

        public void GenerateCodeForStringConstant(StringConstantEntry entry)
        {
            // TODO: Currently obj refs can only be strings
            _currentAssembler.Ld(R16.HL, (entry as StringConstantEntry).Label);
            _currentAssembler.Push(R16.HL);
        }

        public void GenerateCodeForStoreIndirect(StoreIndEntry entry)
        {
            _currentAssembler.Pop(R16.BC);
            _currentAssembler.Pop(R16.HL);
            _currentAssembler.LdInd(R16.HL, R8.C);
        }

        public void GenerateCodeForJumpTrue(JumpTrueEntry entry)
        {
            // Pop i4 from stack and jump if non zero
            _currentAssembler.Pop(R16.HL);
            _currentAssembler.Pop(R16.HL);
            _currentAssembler.Ld(R8.A, 0);
            _currentAssembler.Add(R8.A, R8.L);
            _currentAssembler.Jp(Condition.NonZero, entry.TargetLabel);
        }
        
        public void GenerateCodeForJump(JumpEntry entry)
        {
            _currentAssembler.Jp(entry.TargetLabel);
        }

        public void GenerateCodeForReturn(ReturnEntry entry)
        {
            var targetType = entry.Return;
            var hasReturnValue = targetType != null && targetType.Kind != StackValueKind.Unknown;

            if (hasReturnValue)
            {
                if (targetType.Kind != StackValueKind.Int32)
                {
                    throw new NotImplementedException("Return types other than void and int not supported");
                }

                _currentAssembler.Pop(R16.DE);            // Copy return value into DE
                _currentAssembler.Pop(R16.AF);
            }

            if (_methodCodeNode.LocalsCount > 0)
            {
                _currentAssembler.Ld(R16.SP, I16.IX);     // Move SP to before locals
                _currentAssembler.Pop(I16.IX);            // Remove IX
            }

            if (_methodCodeNode.ParamsCount > 0)
            {
                _currentAssembler.Pop(R16.BC);            // Remove IY
                _currentAssembler.Pop(R16.HL);            // Store return address in HL
                _currentAssembler.Ld(R16.SP, I16.IY);     // Reset SP to before arguments

                _currentAssembler.Push(R16.BC);           // Restore IY
                _currentAssembler.Pop(I16.IY);
                _currentAssembler.Push(R16.HL);           // Restore return address (no args before it now)
            }

            if (hasReturnValue)
            {
                _currentAssembler.Pop(R16.HL);            // Store return address in HL
                _currentAssembler.Push(R16.AF);           // Push return value
                _currentAssembler.Push(R16.DE);
                _currentAssembler.Push(R16.HL);           // Push return address
            }

            _currentAssembler.Ret();
        }

        private static readonly Dictionary<Tuple<Operation, StackValueKind>, string> BinaryOperatorMappings = new()
        {
            { Tuple.Create(Operation.Add, StackValueKind.Int32), "i_add" },
            { Tuple.Create(Operation.Sub, StackValueKind.Int32), "i_sub" },
            { Tuple.Create(Operation.Mul, StackValueKind.Int32), "i_mul" },
            { Tuple.Create(Operation.Div, StackValueKind.Int32), "i_div" },
            { Tuple.Create(Operation.Rem, StackValueKind.Int32), "i_rem" },
            { Tuple.Create(Operation.Div_Un, StackValueKind.Int32), "i_div_un" },
            { Tuple.Create(Operation.Rem_Un, StackValueKind.Int32), "i_rem_un" },
        };

        public void GenerateCodeForBinaryOperator(BinaryOperator entry)
        {
            if (BinaryOperatorMappings.TryGetValue(Tuple.Create(entry.Operation, entry.Kind), out string routine))
            {
                _currentAssembler.Call(routine);
            }
        }

        public void GenerateCodeForNeg(UnaryOperator entry)
        {
            if (entry.Operation == Operation.Neg)
            {
                _currentAssembler.Call("i_neg");
            }
            else
            {
                throw new NotImplementedException($"Unary operator {entry.Operation} not implemented");
            }
        }

        private static readonly Dictionary<Tuple<Operation, StackValueKind>, string> ComparisonOperatorMappings = new()
        {
            { Tuple.Create(Operation.Eq, StackValueKind.Int32), "i_eq" },
            { Tuple.Create(Operation.Ge, StackValueKind.Int32), "i_ge" },
            { Tuple.Create(Operation.Gt, StackValueKind.Int32), "i_gt" },
            { Tuple.Create(Operation.Le, StackValueKind.Int32), "i_le" },
            { Tuple.Create(Operation.Lt, StackValueKind.Int32), "i_lt" },
            { Tuple.Create(Operation.Ne, StackValueKind.Int32), "i_neq" },
        };

        private void GenerateCodeForComparison(BinaryOperator entry)
        {
            if (ComparisonOperatorMappings.TryGetValue(Tuple.Create(entry.Operation, entry.Kind), out string routine))
            {
                _currentAssembler.Call(routine);
                // If carry set then push i4 1 else push i4 0
                _currentAssembler.Ld(R16.HL, 0);
                _currentAssembler.Adc(R16.HL, R16.HL);
                _currentAssembler.Push(R16.HL);
                _currentAssembler.Ld(R16.HL, 0);
                _currentAssembler.Push(R16.HL);
            }
        }

        public void GenerateCodeForLocalVariable(LocalVariableEntry entry)
        {
            if (entry.LocalNumber >= _methodCodeNode.ParamsCount)
            {
                // Loading a local variable
                var localVariable = _localVariableTable[entry.LocalNumber];
                var offset = localVariable.StackOffset;

                _currentAssembler.Ld(R8.H, I16.IX, (short)-(offset + 1));
                _currentAssembler.Ld(R8.L, I16.IX, (short)-(offset + 2));
                _currentAssembler.Push(R16.HL);

                // TODO: Will this always be 4 now?
                if (localVariable.ExactSize == 4)
                {
                    _currentAssembler.Ld(R8.H, I16.IX, (short)-(offset + 3));
                    _currentAssembler.Ld(R8.L, I16.IX, (short)-(offset + 4));
                    _currentAssembler.Push(R16.HL);
                }
            }
            else
            {
                // Loading an argument
                var parameterDescriptor = _localVariableTable[entry.LocalNumber];
                var offset = parameterDescriptor.StackOffset;

                _currentAssembler.Ld(R8.H, I16.IY, (short)-(offset + 1));
                _currentAssembler.Ld(R8.L, I16.IY, (short)-(offset + 2));
                _currentAssembler.Push(R16.HL);

                // TODO: Will this always be 4 now?
                if (parameterDescriptor.ExactSize == 4)
                {
                    _currentAssembler.Ld(R8.H, I16.IY, (short)-(offset + 3));
                    _currentAssembler.Ld(R8.L, I16.IY, (short)-(offset + 4));
                    _currentAssembler.Push(R16.HL);
                }
            }
        }

        public void GenerateCodeForStoreLocalVariable(StoreLocalVariableEntry entry)
        {
            var localVariable = _localVariableTable[entry.LocalNumber];
            var offset = localVariable.StackOffset;

            // TODO: Will this always be 4 now?
            if (localVariable.ExactSize == 4)
            {
                _currentAssembler.Pop(R16.HL);
                _currentAssembler.Ld(I16.IX, (short)-(offset + 3), R8.H);
                _currentAssembler.Ld(I16.IX, (short)-(offset + 4), R8.L);
            }

            _currentAssembler.Pop(R16.HL);
            _currentAssembler.Ld(I16.IX, (short)-(offset + 1), R8.H);
            _currentAssembler.Ld(I16.IX, (short)-(offset + 2), R8.L);
        }

        public void GenerateCodeForCall(CallEntry entry)
        {
            _currentAssembler.Call(entry.TargetMethod);
        }

        public void GenerateCodeForIntrinsic(IntrinsicEntry entry)
        {
            // TODO: Most of this should be done through MethodImplOptions.InternalCall instead
            var methodToCall = entry.TargetMethod;
            switch (methodToCall)
            {
                case "WriteString":
                    _currentAssembler.Pop(R16.HL);    // put argument 1 into HL
                    _currentAssembler.Call("PRINT");
                    break;

                case "WriteInt32":
                    _currentAssembler.Pop(R16.DE);
                    _currentAssembler.Pop(R16.HL);
                    _currentAssembler.Call("LTOA");
                    break;

                case "WriteUInt32":
                    _currentAssembler.Pop(R16.DE);
                    _currentAssembler.Pop(R16.HL);
                    _currentAssembler.Call("ULTOA");
                    break;

                case "WriteChar":
                    _currentAssembler.Pop(R16.DE);    // chars are stored on stack as int32 so remove MSW
                    _currentAssembler.Pop(R16.HL);    // put argument 1 into HL
                    _currentAssembler.Ld(R8.A, R8.L); // Load low byte of argument 1 into A
                    _currentAssembler.Call(0x0033); // ROM routine to display character at current cursor position
                    break;
            }
        }

        public void GenerateCodeForCast(CastEntry entry)
        {
            var actualKind = entry.Op1.Kind;
            var desiredType = entry.DesiredType;
            var unsigned = entry.Unsigned;

            if (actualKind == StackValueKind.Int32 && desiredType == Common.TypeSystem.WellKnownType.UInt16 && unsigned)
            {
                _currentAssembler.Pop(R16.HL);
                _currentAssembler.Pop(R16.DE);

                _currentAssembler.Ld(R16.HL, 0);    // clear msw

                _currentAssembler.Push(R16.DE);
                _currentAssembler.Push(R16.HL);
            }
            else if (actualKind == StackValueKind.Int32 && desiredType == Common.TypeSystem.WellKnownType.Int16 && !unsigned)
            {
                _currentAssembler.Pop(R16.HL);
                _currentAssembler.Pop(R16.DE);

                _currentAssembler.Ld(R8.H, R8.D);

                _currentAssembler.Add(R16.HL, R16.HL);  // move sign bit into carry flag
                _currentAssembler.Sbc(R16.HL, R16.HL);  // hl is now 0 or FFFF

                _currentAssembler.Push(R16.DE);
                _currentAssembler.Push(R16.HL);
            }
            else
            {
                throw new NotImplementedException($"Implicit cast from {actualKind} to {desiredType} not supported");
            }
        }

        private void Optimize(IList<Instruction> instructions)
        {
            EliminatePushXXPopXX(instructions);
        }

        private void EliminatePushXXPopXX(IList<Instruction> instructions)
        {
            int unoptimizedInstructionCount = instructions.Count;
            Instruction lastInstruction = null;
            var currentInstruction = instructions[0];
            int count = 0;
            do
            {
                if (lastInstruction?.Opcode == Opcode.Push && currentInstruction.Opcode == Opcode.Pop
                    && lastInstruction?.Operands == currentInstruction.Operands)
                {
                    // Eliminate Push followed by Pop
                    instructions.RemoveAt(count - 1);
                    instructions.RemoveAt(count - 1);

                    count--;
                    currentInstruction = instructions[count];
                    lastInstruction = count > 0 ? instructions[count - 1] : null;
                }
                else
                {
                    lastInstruction = currentInstruction;
                    if (count + 1 < instructions.Count)
                    {
                        currentInstruction = instructions[++count];
                    }
                }
            } while (count < instructions.Count - 1);

            _compilation.Logger.LogDebug($"Eliminated {unoptimizedInstructionCount - instructions.Count} instructions");
        }
    }
}
