﻿using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.EvaluationStack;
using Z80Assembler;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace ILCompiler.Compiler
{
    // TODO: This shouldn't really have any dependencies on dnlib/IL stuff
    public class CodeGenerator : IStackEntryVisitor
    {
        private readonly Compilation _compilation;
        private readonly LocalVariableDescriptor[] _localVariableTable;

        // TODO: would like to eliminate use of the MethodDef property of _methodCodeNode
        // This is currently used to access the MethodDef which is used for getting:
        //  * count of parameters
        //  * count of local variables
        //  * if method has a non void return type
        private readonly Z80MethodCodeNode _methodCodeNode;

        private readonly Dictionary<string, string> _labelsToStringData = new Dictionary<string, string>();

        private Assembler _currentAssembler = new Assembler();

        public CodeGenerator(Compilation compilation, LocalVariableDescriptor[] localVariableTable, Z80MethodCodeNode methodCodeNodeNeedingCode)
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
            node.Accept(this);
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

            var paramsCount = _methodCodeNode.Method.Parameters.Count;
            if (paramsCount > 0)
            {
                assembler.Push(I16.IY);
                // Set IY to start of arguments here
                // IY = SP - 4 - size of parameters

                assembler.Ld(R16.HL, (short)(4 + parametersSize));
                assembler.Add(R16.HL, R16.SP);
                assembler.Push(R16.HL);
                assembler.Pop(I16.IY);
            }

            var localsCount = _methodCodeNode.Method.Body.Variables.Count;
            if (localsCount > 0)
            {
                assembler.Push(I16.IX);
                assembler.Ld(I16.IX, 0);
                assembler.Add(I16.IX, R16.SP);

                assembler.Ld(R16.HL, (short)-localsSize);
                assembler.Add(R16.HL, R16.SP);
                assembler.Ld(R16.SP, R16.HL);
            }
        }

        public void Visit(Int16ConstantEntry entry)
        {
            var value = (entry as Int16ConstantEntry).Value;
            _currentAssembler.Ld(R16.HL, (short)value);
            _currentAssembler.Push(R16.HL);
        }

        public void Visit(Int32ConstantEntry entry)
        {
            var value = (entry as Int32ConstantEntry).Value;
            var low = BitConverter.ToInt16(BitConverter.GetBytes(value), 0);
            var high = BitConverter.ToInt16(BitConverter.GetBytes(value), 2);

            _currentAssembler.Ld(R16.HL, low);
            _currentAssembler.Push(R16.HL);
            _currentAssembler.Ld(R16.HL, high);
            _currentAssembler.Push(R16.HL);
        }

        public void Visit(StringConstantEntry entry)
        {
            // TODO: Currently obj refs can only be strings
            _currentAssembler.Ld(R16.HL, (entry as StringConstantEntry).Label);
            _currentAssembler.Push(R16.HL);
        }

        public void Visit(StoreIndEntry entry)
        {
            _currentAssembler.Pop(R16.BC);
            _currentAssembler.Pop(R16.HL);
            _currentAssembler.LdInd(R16.HL, R8.C);
        }

        public void Visit(JumpTrueEntry entry)
        {
            _currentAssembler.Jp(Condition.C, entry.TargetLabel);
        }
        
        public void Visit(JumpEntry entry)
        {
            _currentAssembler.Jp(entry.TargetLabel);
        }

        public void Visit(ReturnEntry entry)
        {
            // TODO: This assumes return value if present is int16
            var method = _methodCodeNode.Method;
            var hasReturnValue = method.HasReturnType;
            var hasParameters = method.Parameters.Count > 0;
            var hasLocals = method.Body.Variables.Count > 0;

            if (hasReturnValue)
            {
                _currentAssembler.Pop(R16.DE);            // Copy return value into DE
            }

            if (hasLocals)
            {
                _currentAssembler.Ld(R16.SP, I16.IX);     // Move SP to before locals
                _currentAssembler.Pop(I16.IX);            // Remove IX
            }

            if (hasParameters)
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
                _currentAssembler.Push(R16.DE);           // Push return value
                _currentAssembler.Push(R16.HL);           // Push return address
            }

            _currentAssembler.Ret();
        }

        public void Visit(BinaryOperator entry)
        {
            switch (entry.Op)
            {
                case BinaryOp.ADD:
                    GenerateAdd();
                    break;

                case BinaryOp.SUB:
                    GenerateSub();
                    break;

                case BinaryOp.MUL:
                    GenerateMul();
                    break;

                case BinaryOp.DIV:
                    GenerateDiv();
                    break;

                default:
                    GenerateComparison(entry.Op);
                    break;
            }
        }

        private readonly string[] comparisonRoutinesByOpcode = new string[]
        {
            "s_eq",             // Beq
            "s_ge",             // Bge
            "s_gt",             // Bgt
            "s_le",             // Ble
            "s_lt",             // Blt
            "s_neq"             // Bne
        };

        private void GenerateComparison(BinaryOp op)
        {
            _currentAssembler.Pop(R16.HL);
            _currentAssembler.Pop(R16.DE);

            var comparisonAsmName = comparisonRoutinesByOpcode[op - BinaryOp.EQ];
            _currentAssembler.Call(comparisonAsmName);
        }

        private void GenerateAdd()
        {
            _currentAssembler.Pop(R16.DE);
            _currentAssembler.Pop(R16.HL);
            _currentAssembler.Add(R16.HL, R16.DE);
            _currentAssembler.Push(R16.HL);
        }

        private void GenerateSub()
        {
            _currentAssembler.Pop(R16.DE);
            _currentAssembler.Pop(R16.HL);
            _currentAssembler.Sbc(R16.HL, R16.DE);
            _currentAssembler.Push(R16.HL);
        }

        private void GenerateMul()
        {
            _currentAssembler.Pop(R16.DE);
            _currentAssembler.Pop(R16.BC);
            _currentAssembler.Call("s_mul");
            _currentAssembler.Push(R16.HL);
        }

        private void GenerateDiv()
        {
            _currentAssembler.Pop(R16.DE);
            _currentAssembler.Pop(R16.HL);
            _currentAssembler.Call("s_div");
            _currentAssembler.Push(R16.DE);
        }

        public void Visit(LocalVariableEntry entry)
        {
            if (entry.LocalNumber >= _methodCodeNode.Method.Parameters.Count)
            {
                // Loading a local variable
                var offset = _localVariableTable[entry.LocalNumber].StackOffset;

                _currentAssembler.Ld(R8.H, I16.IX, (short)-(offset + 1));
                _currentAssembler.Ld(R8.L, I16.IX, (short)-(offset + 2));
                _currentAssembler.Push(R16.HL);
            }
            else
            {
                // Loading an argument
                var parameterDescriptor = _localVariableTable[entry.LocalNumber];
                var offset = parameterDescriptor.StackOffset;

                switch (parameterDescriptor.ExactSize)
                {
                    case 2:
                        _currentAssembler.Ld(R8.H, I16.IY, (short)-(offset + 1));
                        _currentAssembler.Ld(R8.L, I16.IY, (short)-(offset + 2));
                        _currentAssembler.Push(R16.HL);
                        break;

                    case 4:
                        _currentAssembler.Ld(R8.H, I16.IY, (short)-(offset + 1));
                        _currentAssembler.Ld(R8.L, I16.IY, (short)-(offset + 2));
                        _currentAssembler.Push(R16.HL);
                        _currentAssembler.Ld(R8.H, I16.IY, (short)-(offset + 3));
                        _currentAssembler.Ld(R8.L, I16.IY, (short)-(offset + 4));
                        _currentAssembler.Push(R16.HL);
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public void Visit(StoreLocalVariableEntry entry)
        {
            // Storing to a local variable
            var offset = entry.LocalNumber * 2; // TODO: This needs to take into account differing sizes of local vars

            _currentAssembler.Pop(R16.HL);
            _currentAssembler.Ld(I16.IX, (short)-(offset + 1), R8.H);
            _currentAssembler.Ld(I16.IX, (short)-(offset + 2), R8.L);
        }

        public void Visit(CallEntry entry)
        {
            _currentAssembler.Call(entry.TargetMethod);
        }

        public void Visit(IntrinsicEntry entry)
        {
            // TODO: Most of this should be done through MethodImplOptions.InternalCall instead
            var methodToCall = entry.TargetMethod;
            switch (methodToCall)
            {
                case "WriteString":
                    _currentAssembler.Pop(R16.HL);    // put argument 1 into HL
                    _currentAssembler.Call("PRINT");
                    break;

                case "WriteInt16":
                    _currentAssembler.Pop(R16.HL);    // put argument 1 into HL
                    _currentAssembler.Call("NUM2DEC2");
                    break;

                case "WriteInt32":
                    _currentAssembler.Pop(R16.DE);
                    _currentAssembler.Pop(R16.HL);
                    _currentAssembler.Call("LTOA");
                    break;

                case "WriteChar":
                    _currentAssembler.Pop(R16.HL);    // put argument 1 into HL
                    _currentAssembler.Ld(R8.A, R8.L); // Load low byte of argument 1 into A
                    _currentAssembler.Call(0x0033); // ROM routine to display character at current cursor position
                    break;
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

            _compilation.Logger.LogInformation($"Eliminated {unoptimizedInstructionCount - instructions.Count} instructions");
        }
    }
}
