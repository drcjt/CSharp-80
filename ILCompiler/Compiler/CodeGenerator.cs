using ILCompiler.Common.TypeSystem;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.EvaluationStack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Z80Assembler;

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
            AssignFrameOffsets();

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

        private void AssignFrameOffsets()
        {
            // First process the arguments
            var totalArgumentsSize = AssignFrameOffsetsToArgs();

            // Calculate the frame offsets for local variables
            AssignFrameOffsetsToLocals();

            // Patch the offsets
            FixFrameOffsets(totalArgumentsSize);
        }

        private void FixFrameOffsets(int totalArgumentsSize)
        {
            // Now fixup offset for parameters given that we now know the overall size of the parameters
            // and also take into account the return address and frame pointer (IX)
            foreach (var variable in _localVariableTable)
            {
                if (variable.IsParameter)
                {
                    // return address occupies 2 bytes
                    // frame pointer occupies 2 bytes
                    variable.StackOffset -= (totalArgumentsSize + 2 + 2);
                }
            }
        }

        private int AssignFrameOffsetsToArgs()
        {
            var totalArgumentsSize = 0;
            var offset = 0;

            foreach (var localVariable in _localVariableTable)
            {
                if (localVariable.IsParameter)
                {
                    var argumentSize = localVariable.ExactSize;
                    localVariable.ExactSize = argumentSize;
                    localVariable.StackOffset = offset + localVariable.ExactSize;

                    offset += argumentSize;
                    totalArgumentsSize += argumentSize;
                }
            }

            return totalArgumentsSize;
        }

        private void AssignFrameOffsetsToLocals()
        {
            var offset = 0;
            foreach (var localVariable in _localVariableTable)
            {
                if (!localVariable.IsParameter)
                {
                    localVariable.StackOffset = offset + localVariable.ExactSize;
                    offset += localVariable.ExactSize;
                }
            }
        }

        private void GenerateFromNode(StackEntry node)
        {
            switch (node.Operation)
            {
                case Operation.StoreIndirect:
                    GenerateCodeForStoreIndirect(node.As<StoreIndEntry>());
                    break;

                case Operation.Return:
                    GenerateCodeForReturn(node.As<ReturnEntry>());
                    break;

                case Operation.Constant_Int32:
                    GenerateCodeForInt32Constant(node.As<Int32ConstantEntry>());
                    break;

                case Operation.Constant_String:
                    GenerateCodeForStringConstant(node.As<StringConstantEntry>());
                    break;

                case Operation.JumpTrue:
                    GenerateCodeForJumpTrue(node.As<JumpTrueEntry>());
                    break;

                case Operation.Jump:
                    GenerateCodeForJump(node.As<JumpEntry>());
                    break;

                case Operation.Neg:
                    GenerateCodeForNeg(node.As<UnaryOperator>());
                    break;

                case Operation.Eq:
                case Operation.Ne:
                case Operation.Lt:
                case Operation.Le:
                case Operation.Gt:
                case Operation.Ge:
                    GenerateCodeForComparison(node.As<BinaryOperator>());
                    break;

                case Operation.Add:
                case Operation.Mul:
                case Operation.Sub:
                case Operation.Div:
                case Operation.Rem:
                case Operation.Div_Un:
                case Operation.Rem_Un:
                    GenerateCodeForBinaryOperator(node.As<BinaryOperator>());
                    break;

                case Operation.LocalVariable:
                    GenerateCodeForLocalVariable(node.As<LocalVariableEntry>());
                    break;

                case Operation.LocalVariableAddress:
                    GenerateCodeForLocalVariableAddress(node.As<LocalVariableAddressEntry>());
                    break;

                case Operation.StoreLocalVariable:
                    GenerateCodeForStoreLocalVariable(node.As<StoreLocalVariableEntry>());
                    break;

                case Operation.Field:
                    GenerateCodeForField(node.As<FieldEntry>());
                    break;

                case Operation.FieldAddress:
                    GenerateCodeForFieldAddress(node.As<FieldAddressEntry>());
                    break;

                case Operation.Indirect:
                    GenerateCodeForIndirect(node.As<IndirectEntry>());
                    break;

                case Operation.Call:
                    GenerateCodeForCall(node.As<CallEntry>());
                    break;

                case Operation.Intrinsic:
                    GenerateCodeForIntrinsic(node.As<IntrinsicEntry>());
                    break;

                case Operation.Cast:
                    GenerateCodeForCast(node.As<CastEntry>());
                    break;

                case Operation.Switch:
                    GenerateCodeForSwitch(node.As<SwitchEntry>());
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
                        var stringConstantEntry = currentNode.As<StringConstantEntry>();

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
            //     |-----------------------|
            //     |       incoming        |
            //     |       arguments       |
            //     |-----------------------|
            //     |    return address     |
            //     +=======================+
            //     |     IX (optional)     |    Not present if no locals or params
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

            var localsSize = 0;
            var tempCount = 0;
            foreach (var localVariable in _localVariableTable)
            {
                if (localVariable.IsTemp)
                {
                    tempCount++;
                }
                if (!localVariable.IsParameter)
                {
                    localsSize += localVariable.ExactSize;
                }
            }

            if (_methodCodeNode.ParamsCount > 0 || (_methodCodeNode.LocalsCount + tempCount) > 0)
            {
                assembler.Push(I16.IX);
                assembler.Ld(I16.IX, 0);
                assembler.Add(I16.IX, R16.SP);
            }

            if (_methodCodeNode.LocalsCount + tempCount > 0)
            {
                // Reserve space on stack for locals
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
            _currentAssembler.Ld(R16.HL, 0);
            _currentAssembler.Push(R16.HL);
        }

        public void GenerateCodeForStoreIndirect(StoreIndEntry entry)
        {
            var exactSize = entry.ExactSize ?? 0;
            if (exactSize > 0)
            {
                _currentAssembler.Pop(R16.HL);
                _currentAssembler.Pop(R16.HL);  // Address is stored as 32 bits but will only use lsw

                _currentAssembler.Push(I16.IX);
                _currentAssembler.Pop(R16.BC);

                _currentAssembler.Push(R16.HL);
                _currentAssembler.Pop(I16.IX);

                short offset = (short)entry.FieldOffset;                
                CopyFromStackToIX(exactSize, offset);

                _currentAssembler.Push(R16.BC);
                _currentAssembler.Pop(I16.IX);
            }
        }

        private void CopyFromStackToIX(int size, int ixOffset = 0, bool restoreIX = false)
        {
            int changeToIX = 0;

            var totalBytesToCopy = size;
            int originalIxOffset = ixOffset;
            do
            {
                var bytesToCopy = totalBytesToCopy > 4 ? 4 : totalBytesToCopy;

                // offset has to be -128 to + 127
                if (ixOffset + 3 > 127)
                {
                    // Need to move IX along to keep stackOffset within -128 to +127 range
                    _currentAssembler.Ld(R16.DE, 127);
                    _currentAssembler.Add(I16.IX, R16.DE);
                    changeToIX += 127;

                    ixOffset -= 127;
                    size -= 127;
                }

                switch (bytesToCopy)
                {
                    case 1:
                        _currentAssembler.Pop(R16.HL);
                        _currentAssembler.Pop(R16.HL);
                        _currentAssembler.Ld(I16.IX, (short)(ixOffset + 0), R8.L);
                        break;
                    case 2:
                        _currentAssembler.Pop(R16.HL);
                        _currentAssembler.Pop(R16.HL);
                        _currentAssembler.Ld(I16.IX, (short)(ixOffset + 1), R8.H);
                        _currentAssembler.Ld(I16.IX, (short)(ixOffset + 0), R8.L);
                        break;
                    case 4:
                        _currentAssembler.Pop(R16.HL);
                        _currentAssembler.Ld(I16.IX, (short)(ixOffset + 1), R8.H);
                        _currentAssembler.Ld(I16.IX, (short)(ixOffset + 0), R8.L);
                        _currentAssembler.Pop(R16.HL);
                        _currentAssembler.Ld(I16.IX, (short)(ixOffset + 3), R8.H);
                        _currentAssembler.Ld(I16.IX, (short)(ixOffset + 2), R8.L);
                        break;
                }

                ixOffset += 4;
                totalBytesToCopy -= 4;
            } while (ixOffset < size + originalIxOffset);

            if (changeToIX != 0 && restoreIX)
            {
                _currentAssembler.Ld(R16.DE, (short)(-changeToIX));
                _currentAssembler.Add(I16.IX, R16.DE);
            }
        }

        private void CopyFromIXToStack(int size, int ixOffset = 0, bool restoreIX = false)
        {
            int changeToIX = 0;

            int originalIxOffset = ixOffset;
            ixOffset += size - 4;
            do
            {
                var bytesToCopy = size > 4 ? 4 : size;
                size -= 4;

                if (ixOffset + 3 < -128)
                {
                    var delta = ixOffset + 3;
                    _currentAssembler.Ld(R16.DE, (short)delta);
                    _currentAssembler.Add(I16.IX, R16.DE);
                    changeToIX += delta;

                    ixOffset -= delta;
                    originalIxOffset -= delta;
                }

                switch (bytesToCopy)
                {
                    case 1:
                        _currentAssembler.Ld(R8.H, 0);
                        _currentAssembler.Ld(R8.L, I16.IX, (short)(ixOffset + 3));
                        _currentAssembler.Push(R16.HL);
                        _currentAssembler.Ld(R16.HL, 0);
                        _currentAssembler.Push(R16.HL);
                        break;

                    case 2:
                        _currentAssembler.Ld(R8.H, I16.IX, (short)(ixOffset + 3));
                        _currentAssembler.Ld(R8.L, I16.IX, (short)(ixOffset + 2));
                        _currentAssembler.Push(R16.HL);
                        _currentAssembler.Ld(R16.HL, 0);
                        _currentAssembler.Push(R16.HL);
                        break;

                    case 4:
                        _currentAssembler.Ld(R8.H, I16.IX, (short)(ixOffset + 3));
                        _currentAssembler.Ld(R8.L, I16.IX, (short)(ixOffset + 2));
                        _currentAssembler.Push(R16.HL);
                        _currentAssembler.Ld(R8.H, I16.IX, (short)(ixOffset + 1));
                        _currentAssembler.Ld(R8.L, I16.IX, (short)(ixOffset + 0));
                        _currentAssembler.Push(R16.HL);
                        break;
                }

                ixOffset -= 4;
            } while (ixOffset >= originalIxOffset);

            if (changeToIX != 0 && restoreIX)
            {
                _currentAssembler.Ld(R16.DE, (short)(-changeToIX));
                _currentAssembler.Add(I16.IX, R16.DE);
            }
        }

        public void GenerateCodeForField(FieldEntry entry)
        {
            // Load field onto stack
            var fieldOffset = entry.Offset ?? 0;
            var exactSize = entry.ExactSize ?? 0;

            var indirectEntry = new IndirectEntry(entry, entry.Kind, exactSize);
            GenerateCodeForIndirect(indirectEntry, fieldOffset, exactSize);
        }

        public void GenerateCodeForFieldAddress(FieldAddressEntry entry)
        {
            var fieldOffset = entry.Offset;

            // Get address of object
            _currentAssembler.Pop(R16.DE);      // lsw will be ignored
            _currentAssembler.Pop(R16.HL);

            // Calculate field address
            _currentAssembler.Ld(R16.DE, (short)fieldOffset);
            _currentAssembler.Add(R16.HL, R16.DE);

            // Push field address onto the stack
            _currentAssembler.Push(R16.HL);
            _currentAssembler.Ld(R16.DE, 0);
            _currentAssembler.Push(R16.DE);
        }

        public void GenerateCodeForIndirect(IndirectEntry entry, uint fieldOffset = 0, int fieldSize = 4)
        {
            if (entry.Kind == StackValueKind.Int32 || entry.Kind == StackValueKind.ValueType || entry.Kind == StackValueKind.NativeInt)
            {
                // Save IX into BC
                _currentAssembler.Push(I16.IX);
                _currentAssembler.Pop(R16.BC);

                // Get indirect address from stack into IX
                _currentAssembler.Pop(I16.IX);  // Ignore lsw of address
                _currentAssembler.Pop(I16.IX);

                var size = entry.ExactSize ?? 4;
                CopyFromIXToStack(size, (short)fieldOffset);

                // Restore IX from BC
                _currentAssembler.Push(R16.BC);
                _currentAssembler.Pop(I16.IX);
            }
            else
            {
                throw new NotImplementedException();
            }
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

        public void GenerateCodeForSwitch(SwitchEntry entry)
        {
            _currentAssembler.Pop(R16.HL);
            _currentAssembler.Pop(R16.HL);

            _currentAssembler.Ld(R8.A, R8.L);

            for (int targetIndex = 0; targetIndex < entry.JumpTable.Count; targetIndex++)
            {
                _currentAssembler.Or(R8.A);
                _currentAssembler.Jp(Condition.Zero, entry.JumpTable[targetIndex]);

                if (targetIndex < entry.JumpTable.Count - 1)
                {
                    _currentAssembler.Dec(R8.A);
                }
            }
        }

        public void GenerateCodeForReturn(ReturnEntry entry)
        {
            var targetType = entry.Return;
            var hasReturnValue = targetType != null && targetType.Kind != StackValueKind.Unknown;

            if (hasReturnValue)
            {
                if (entry.ReturnBufferArgIndex.HasValue)
                {
                    // Returning a struct

                    // Load address of return buffer into HL
                    var variable = _localVariableTable[entry.ReturnBufferArgIndex.Value];
                    _currentAssembler.Ld(R8.H, I16.IX, (short)-(variable.StackOffset - 3));
                    _currentAssembler.Ld(R8.L, I16.IX, (short)-(variable.StackOffset - 2));

                    _currentAssembler.Push(I16.IX); // save IX to BC
                    _currentAssembler.Pop(R16.BC);

                    _currentAssembler.Push(R16.HL); // Move HL to IX
                    _currentAssembler.Pop(I16.IX);

                    // Copy struct to the return buffer
                    var returnTypeExactSize = entry.ReturnTypeExactSize ?? 0;
                    CopyFromStackToIX(returnTypeExactSize);

                    _currentAssembler.Push(R16.BC); // restore IX
                    _currentAssembler.Pop(I16.IX);
                }
                else if (targetType?.Kind != StackValueKind.Int32)
                {
                    throw new NotImplementedException($"Unsupported return type {targetType?.Kind}");
                }
                else
                {
                    _currentAssembler.Pop(R16.DE);            // Copy return value into DE/IY
                    _currentAssembler.Pop(I16.IY);
                }
            }

            // Unwind stack frame
            var tempCount = 0;
            foreach (var localVariable in _localVariableTable)
            {
                if (localVariable.IsTemp)
                {
                    tempCount++;
                }
            }

            if (_methodCodeNode.ParamsCount > 0 || (_methodCodeNode.LocalsCount + tempCount) > 0)
            {
                if (_methodCodeNode.LocalsCount + tempCount > 0)
                {
                    _currentAssembler.Ld(R16.SP, I16.IX);     // Move SP to before locals
                }
                _currentAssembler.Pop(I16.IX);            // Remove IX

                if (_methodCodeNode.ParamsCount > 0)
                {
                    // Calculate size of parameters
                    var totalParametersSize = 0;
                    foreach (var local in _localVariableTable)
                    {
                        if (local.IsParameter)
                        {
                            totalParametersSize += local.ExactSize;
                        }
                    }

                    // TODO: consider optimising simple cases to just use Pop to remove the parameters.
                    // will probably be better for 1 or maybe 2 32bit parameters.

                    // Work out start of params so we can reset SP after removing return address
                    _currentAssembler.Ld(R16.HL, 0);
                    _currentAssembler.Add(R16.HL, R16.SP);
                    _currentAssembler.Ld(R16.BC, (short)(2 + totalParametersSize));
                    _currentAssembler.Add(R16.HL, R16.BC);
                }

                _currentAssembler.Pop(R16.BC);      // Store return address in BC

                if (_methodCodeNode.ParamsCount > 0)
                {
                    // Remove parameters from stack
                    _currentAssembler.Ld(R16.SP, R16.HL);
                }
            }
            else
            {
                _currentAssembler.Pop(R16.BC);      // Store return address in BC
            }

            if (hasReturnValue && !entry.ReturnBufferArgIndex.HasValue)
            {
                _currentAssembler.Push(I16.IY);
                _currentAssembler.Push(R16.DE);
            }

            _currentAssembler.Push(R16.BC);
            _currentAssembler.Ret();
        }

        private static readonly Dictionary<Tuple<Operation, StackValueKind>, string> BinaryOperatorMappings = new()
        {
            { Tuple.Create(Operation.Add, StackValueKind.Int32), "i_add" },
            { Tuple.Create(Operation.Add, StackValueKind.NativeInt), "i_add" },
            { Tuple.Create(Operation.Sub, StackValueKind.Int32), "i_sub" },
            { Tuple.Create(Operation.Mul, StackValueKind.Int32), "i_mul" },
            { Tuple.Create(Operation.Div, StackValueKind.Int32), "i_div" },
            { Tuple.Create(Operation.Rem, StackValueKind.Int32), "i_rem" },
            { Tuple.Create(Operation.Div_Un, StackValueKind.Int32), "i_div_un" },
            { Tuple.Create(Operation.Rem_Un, StackValueKind.Int32), "i_rem_un" },
        };

        public void GenerateCodeForBinaryOperator(BinaryOperator entry)
        {
            if (BinaryOperatorMappings.TryGetValue(Tuple.Create(entry.Operation, entry.Kind), out string? routine))
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
            if (ComparisonOperatorMappings.TryGetValue(Tuple.Create(entry.Operation, entry.Kind), out string? routine))
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
            var variable = _localVariableTable[entry.LocalNumber];

            // Loading a local variable/argument
            Debug.Assert(variable.ExactSize % 4 == 0);
            CopyFromIXToStack(variable.ExactSize, -variable.StackOffset, restoreIX: true);
        }

        public void GenerateCodeForStoreLocalVariable(StoreLocalVariableEntry entry)
        {
            var variable = _localVariableTable[entry.LocalNumber];

            // Storing a local variable/argument
            Debug.Assert(variable.ExactSize % 4 == 0);
            CopyFromStackToIX(variable.ExactSize, -variable.StackOffset, restoreIX: true);
        }

        public void GenerateCodeForLocalVariableAddress(LocalVariableAddressEntry entry)
        {
            // Loading address of a local variable/argument
            var localVariable = _localVariableTable[entry.LocalNumber];
            var offset = localVariable.StackOffset;

            // Calculate and push the actual 16 bit address
            _currentAssembler.Push(I16.IX);
            _currentAssembler.Pop(R16.HL);

            _currentAssembler.Ld(R16.DE, (short)(-offset));
            _currentAssembler.Add(R16.HL, R16.DE);

            // Push address
            _currentAssembler.Push(R16.HL);

            // Push 0 to makeup full 32 bit value
            _currentAssembler.Ld(R16.HL, 0);
            _currentAssembler.Push(R16.HL);
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
                    _currentAssembler.Pop(R16.DE);    // put argument 1 into HL
                    _currentAssembler.Pop(R16.HL);
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

            if (actualKind == StackValueKind.Int32 && desiredType == Common.TypeSystem.WellKnownType.UInt16)
            {
                _currentAssembler.Pop(R16.HL);
                _currentAssembler.Pop(R16.DE);

                _currentAssembler.Ld(R16.HL, 0);    // clear msw

                _currentAssembler.Push(R16.DE);
                _currentAssembler.Push(R16.HL);
            }
            else if (actualKind == StackValueKind.Int32 && desiredType == Common.TypeSystem.WellKnownType.Int16)
            {
                _currentAssembler.Pop(R16.HL);
                _currentAssembler.Pop(R16.DE);

                _currentAssembler.Ld(R8.H, R8.D);

                _currentAssembler.Add(R16.HL, R16.HL);  // move sign bit into carry flag
                _currentAssembler.Sbc(R16.HL, R16.HL);  // hl is now 0 or FFFF

                _currentAssembler.Push(R16.DE);
                _currentAssembler.Push(R16.HL);
            }
            else if (actualKind == StackValueKind.Int32 && desiredType == WellKnownType.Byte)
            {
                _currentAssembler.Pop(R16.HL);
                _currentAssembler.Pop(R16.DE);

                _currentAssembler.Ld(R16.HL, 0);    // clear msw
                _currentAssembler.Ld(R8.D, 0);

                _currentAssembler.Push(R16.DE);
                _currentAssembler.Push(R16.HL);
            }
            else if (actualKind == StackValueKind.Int32 && desiredType == WellKnownType.SByte)
            {
                _currentAssembler.Pop(R16.HL);
                _currentAssembler.Pop(R16.DE);

                _currentAssembler.Ld(R8.H, R8.E);

                _currentAssembler.Add(R16.HL, R16.HL);  // move sign bit into carry flag
                _currentAssembler.Sbc(R16.HL, R16.HL);  // hl is now 0000 or FFFF
                _currentAssembler.Ld(R8.D, R8.L);       // D is now 00 or FF

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
            Instruction? lastInstruction = null;
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
