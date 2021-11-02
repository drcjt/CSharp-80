using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.CodeGenerators;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.EvaluationStack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Z80Assembler;

namespace ILCompiler.Compiler
{
    public class CodeGenerator : IStackEntryVisitor
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
                    currentNode.Accept(this);
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

        public void Visit(Int32ConstantEntry entry)
        {
            Int32ConstantCodeGenerator.GenerateCode(entry, _currentAssembler);
        }

        public void Visit(StringConstantEntry entry)
        {
            StringConstantCodeGenerator.GenerateCode(entry, _currentAssembler);
        }

        public void Visit(StoreIndEntry entry)
        {
            GenerateCodeForStoreIndirect(entry);
        }

        public void Visit(JumpTrueEntry entry)
        {
            JumpTrueCodeGenerator.GenerateCode(entry, _currentAssembler);
        }

        public void Visit(JumpEntry entry)
        {
            JumpCodeGenerator.GenerateCode(entry, _currentAssembler);
        }

        public void Visit(ReturnEntry entry)
        {
            GenerateCodeForReturn(entry);
        }

        public void Visit(BinaryOperator entry)
        {
            if (entry.IsComparison)
            {
                ComparisonCodeGenerator.GenerateCode(entry, _currentAssembler);
            }
            else
            {
                BinaryOperatorCodeGenerator.GenerateCode(entry, _currentAssembler);
            }
        }

        public void Visit(LocalVariableEntry entry)
        {
            GenerateCodeForLocalVariable(entry);
        }

        public void Visit(LocalVariableAddressEntry entry)
        {
            LocalVariableAddressCodeGenerator.GenerateCode(entry, _currentAssembler, _localVariableTable);
        }

        public void Visit(StoreLocalVariableEntry entry)
        {
            GenerateCodeForStoreLocalVariable(entry);
        }

        public void Visit(CallEntry entry)
        {
            CallCodeGenerator.GenerateCode(entry, _currentAssembler);
        }

        public void Visit(IntrinsicEntry entry)
        {
            IntrinsicCodeGenerator.GenerateCode(entry, _currentAssembler);
        }

        public void Visit(CastEntry entry)
        {
            CastCodeGenerator.GenerateCode(entry, _currentAssembler);
        }

        public void Visit(UnaryOperator entry)
        {
            UnaryOperatorCodeGenerator.GenerateCode(entry, _currentAssembler);
        }

        public void Visit(IndirectEntry entry)
        {
            GenerateCodeForIndirect(entry);
        }

        public void Visit(FieldEntry entry)
        {
            GenerateCodeForField(entry);
        }

        public void Visit(FieldAddressEntry entry)
        {
            FieldAddressCodeGenerator.GenerateCode(entry, _currentAssembler);
        }

        public void Visit(SwitchEntry entry)
        {
            SwitchCodeGenerator.GenerateCode(entry, _currentAssembler);
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
