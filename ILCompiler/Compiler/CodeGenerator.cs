using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.z80;
using System;
using System.Collections.Generic;

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

        private IList<Instruction> _currentBlockInstructions = new List<Instruction>();

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
            GenerateStringData();

            Append(new LabelInstruction(_compilation.NameMangler.GetMangledMethodName(_methodCodeNode.Method)));

            GenerateProlog();
            methodInstructions.AddRange(_currentBlockInstructions);

            foreach (var block in blocks)
            {
                _currentBlockInstructions.Clear();

                Append(new LabelInstruction(block.Label));

                var currentNode = block.FirstNode;
                while (currentNode != null)
                {
                    GenerateFromNode(currentNode);
                    currentNode = currentNode.Next;
                }

                _compilation.Optimizer.Optimize(_currentBlockInstructions);
                methodInstructions.AddRange(_currentBlockInstructions);
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

        private void GenerateStringData()
        {
            // TODO: Need to eliminate duplicate strings
            foreach (var keyValuePair in _labelsToStringData)
            {
                Append(new LabelInstruction(keyValuePair.Key));
                foreach (var ch in keyValuePair.Value)
                {
                    Append(Instruction.Db((byte)ch));
                }
                Append(Instruction.Db(0));
            }
        }
        private void GenerateProlog()
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
                Append(Instruction.Push(I16.IY));
                // Set IY to start of arguments here
                // IY = SP - 4 - size of parameters

                Append(Instruction.Ld(R16.HL, (short)(4 + parametersSize)));
                Append(Instruction.Add(R16.HL, R16.SP));
                Append(Instruction.Push(R16.HL));
                Append(Instruction.Pop(I16.IY));
            }

            var localsCount = _methodCodeNode.Method.Body.Variables.Count;
            if (localsCount > 0)
            {
                Append(Instruction.Push(I16.IX));
                Append(Instruction.Ld(I16.IX, 0));
                Append(Instruction.Add(I16.IX, R16.SP));

                Append(Instruction.Ld(R16.HL, (short)-localsSize));
                Append(Instruction.Add(R16.HL, R16.SP));
                Append(Instruction.Ld(R16.SP, R16.HL));
            }
        }

        public void Visit(Int16ConstantEntry entry)
        {
            var value = (entry as Int16ConstantEntry).Value;
            Append(Instruction.Ld(R16.HL, (short)value));
            Append(Instruction.Push(R16.HL));
        }

        public void Visit(Int32ConstantEntry entry)
        {
            var value = (entry as Int32ConstantEntry).Value;
            var low = BitConverter.ToInt16(BitConverter.GetBytes(value), 0);
            var high = BitConverter.ToInt16(BitConverter.GetBytes(value), 2);

            Append(Instruction.Ld(R16.HL, low));
            Append(Instruction.Push(R16.HL));
            Append(Instruction.Ld(R16.HL, high));
            Append(Instruction.Push(R16.HL));
        }

        public void Visit(StringConstantEntry entry)
        {
            // TODO: Currently obj refs can only be strings
            Append(Instruction.Ld(R16.HL, (entry as StringConstantEntry).Label));
            Append(Instruction.Push(R16.HL));
        }

        public void Visit(StoreIndEntry entry)
        {
            Append(Instruction.Pop(R16.BC));
            Append(Instruction.Pop(R16.HL));
            Append(Instruction.LdInd(R16.HL, R8.C));
        }

        public void Visit(JumpTrueEntry entry)
        {
            Append(Instruction.Jp(Condition.C, entry.TargetLabel));
        }
        
        public void Visit(JumpEntry entry)
        {
            Append(Instruction.Jp(entry.TargetLabel));
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
                Append(Instruction.Pop(R16.DE));            // Copy return value into DE
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
                Append(Instruction.Push(R16.HL));           // Restore return address (no args before it now)
            }

            if (hasReturnValue)
            {
                Append(Instruction.Pop(R16.HL));            // Store return address in HL
                Append(Instruction.Push(R16.DE));           // Push return value
                Append(Instruction.Push(R16.HL));           // Push return address
            }
            else if (hasParameters)
            {
                Append(Instruction.Push(R16.HL));           // Push return address
            }

            Append(Instruction.Ret());
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
            Append(Instruction.Pop(R16.HL));
            Append(Instruction.Pop(R16.DE));

            var comparisonAsmName = comparisonRoutinesByOpcode[op - BinaryOp.EQ];
            Append(Instruction.Call(comparisonAsmName));
        }

        private void GenerateAdd()
        {
            Append(Instruction.Pop(R16.DE));
            Append(Instruction.Pop(R16.HL));
            Append(Instruction.Add(R16.HL, R16.DE));
            Append(Instruction.Push(R16.HL));
        }

        private void GenerateSub()
        {
            Append(Instruction.Pop(R16.DE));
            Append(Instruction.Pop(R16.HL));
            Append(Instruction.Sbc(R16.HL, R16.DE));
            Append(Instruction.Push(R16.HL));
        }

        private void GenerateMul()
        {
            Append(Instruction.Pop(R16.DE));
            Append(Instruction.Pop(R16.BC));
            Append(Instruction.Call("s_mul"));
            Append(Instruction.Push(R16.HL));
        }

        public void Visit(LocalVariableEntry entry)
        {
            if (entry.LocalNumber >= _methodCodeNode.Method.Parameters.Count)
            {
                // Loading a local variable
                var offset = _localVariableTable[entry.LocalNumber].StackOffset;

                Append(Instruction.Ld(R8.H, I16.IX, (short)-(offset + 1)));
                Append(Instruction.Ld(R8.L, I16.IX, (short)-(offset + 2)));
                Append(Instruction.Push(R16.HL));
            }
            else
            {
                // Loading an argument
                var parameterDescriptor = _localVariableTable[entry.LocalNumber];
                var offset = parameterDescriptor.StackOffset;

                switch (parameterDescriptor.ExactSize)
                {
                    case 2:
                        Append(Instruction.Ld(R8.H, I16.IY, (short)-(offset + 1)));
                        Append(Instruction.Ld(R8.L, I16.IY, (short)-(offset + 2)));
                        Append(Instruction.Push(R16.HL));
                        break;

                    case 4:
                        Append(Instruction.Ld(R8.H, I16.IY, (short)-(offset + 1)));
                        Append(Instruction.Ld(R8.L, I16.IY, (short)-(offset + 2)));
                        Append(Instruction.Push(R16.HL));
                        Append(Instruction.Ld(R8.H, I16.IY, (short)-(offset + 3)));
                        Append(Instruction.Ld(R8.L, I16.IY, (short)-(offset + 4)));
                        Append(Instruction.Push(R16.HL));
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

            Append(Instruction.Pop(R16.HL));
            Append(Instruction.Ld(I16.IX, (short)-(offset + 1), R8.H));
            Append(Instruction.Ld(I16.IX, (short)-(offset + 2), R8.L));
        }

        public void Visit(CallEntry entry)
        {
            Append(Instruction.Call(entry.TargetMethod));
        }

        public void Visit(IntrinsicEntry entry)
        {
            // TODO: Most of this should be done through MethodImplOptions.InternalCall instead
            var methodToCall = entry.TargetMethod;
            switch (methodToCall)
            {
                case "WriteString":
                    Append(Instruction.Pop(R16.HL));    // put argument 1 into HL
                    Append(Instruction.Call("PRINT"));
                    break;

                case "WriteInt16":
                    Append(Instruction.Pop(R16.HL));    // put argument 1 into HL
                    Append(Instruction.Call("NUM2DEC2"));
                    break;

                // TODO: LTOA only prints unsigned ints at the minute
                // need to fix z80 code to test for -ve numbers, print -ve sign
                // and negate number before calling LTOA
                case "WriteInt32":
                    Append(Instruction.Pop(R16.DE));
                    Append(Instruction.Pop(R16.HL));
                    Append(Instruction.Call("LTOA"));
                    break;

                case "WriteChar":
                    Append(Instruction.Pop(R16.HL));    // put argument 1 into HL
                    Append(Instruction.Ld(R8.A, R8.L)); // Load low byte of argument 1 into A
                    Append(Instruction.Call(0x0033)); // ROM routine to display character at current cursor position
                    break;
            }
        }

        public void Append(Instruction instruction)
        {
            _currentBlockInstructions.Add(instruction);
        }
    }
}
