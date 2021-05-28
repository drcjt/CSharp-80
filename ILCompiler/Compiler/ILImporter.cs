using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.z80;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Instruction = ILCompiler.z80.Instruction;

namespace ILCompiler.Compiler
{
    public class ILImporter
    {
        private readonly Compilation _compilation;     // TODO: Is this required
        private readonly Z80Writer _writer;             // TODO: Is this required
        private readonly MethodDef _method;

        private BasicBlock[] _basicBlocks;

        private BasicBlock _pendingBasicBlocks;

        private class BasicBlock
        {
            public BasicBlock Next;
            public int StartOffset;
        }

        private List<Instruction> _instructions = new List<Instruction>();

        public ILImporter(Compilation compilation, Z80Writer writer, MethodDef method)
        {
            _compilation = compilation;
            _writer = writer;
            _method = method;
        }

        public void Compile(Z80MethodCodeNode methodCodeNodeNeedingCode)
        {
            var offsetToIndexMap = FindBasicBlocks();    // This finds the basic blocks

            ImportBasicBlocks(offsetToIndexMap);  // This converts IL to Z80

            methodCodeNodeNeedingCode.SetCode(_instructions);
        }

        private IDictionary<int, int> FindBasicBlocks()
        {
            var instructions = _method.Body.Instructions;
            var lastInstruction = instructions[instructions.Count - 1];
            var maxOffset = (int)lastInstruction.Offset + lastInstruction.GetSize();
            _basicBlocks = new BasicBlock[maxOffset];

            CreateBasicBlock(0);

            return FindJumpTargets(maxOffset);
        }

        private BasicBlock CreateBasicBlock(int offset)
        {
            BasicBlock basicBlock = _basicBlocks[offset];
            if (basicBlock == null)
            {
                basicBlock = new BasicBlock() { StartOffset = offset };
                _basicBlocks[offset] = basicBlock;
            }

            return basicBlock;
        }

        private IDictionary<int, int> FindJumpTargets(int maxOffset)
        {
            var currentIndex = 0;
            var currentOffset = 0;

            var offsetToIndexMap = new Dictionary<int, int>();

            while (currentIndex < _method.Body.Instructions.Count)
            {
                offsetToIndexMap[currentOffset] = currentIndex;
                var currentInstruction = _method.Body.Instructions[currentIndex];

                switch (currentInstruction.OpCode.Code)
                {
                    case Code.Blt_Un:
                    case Code.Ble_Un:
                    case Code.Bgt_Un:
                    case Code.Bge_Un:
                    case Code.Bne_Un:
                    case Code.Blt:
                    case Code.Ble:
                    case Code.Bgt:
                    case Code.Bge:
                    case Code.Beq:
                    case Code.Brfalse:
                    case Code.Brtrue:
                    case Code.Blt_Un_S:
                    case Code.Ble_Un_S:
                    case Code.Bgt_Un_S:
                    case Code.Bge_Un_S:
                    case Code.Bne_Un_S:
                    case Code.Blt_S:
                    case Code.Ble_S:
                    case Code.Bgt_S:
                    case Code.Bge_S:
                    case Code.Beq_S:
                    case Code.Brfalse_S:
                    case Code.Brtrue_S:
                        {
                            var target = currentInstruction.Operand as dnlib.DotNet.Emit.Instruction;
                            var targetOffset = target.Offset;
                            CreateBasicBlock((int)targetOffset); // target of jump                            
                            var nextInstructionOffset = currentOffset + currentInstruction.GetSize();
                            CreateBasicBlock(nextInstructionOffset); // instruction after jump
                        }
                        break;

                    case Code.Br_S:
                    case Code.Leave_S:
                    case Code.Br:
                    case Code.Leave:
                        {
                            var target = currentInstruction.Operand as dnlib.DotNet.Emit.Instruction;
                            var targetOffset = target.Offset;
                            CreateBasicBlock((int)targetOffset); // target of jump
                        }
                        break;

                    case Code.Switch:
                        {
                            throw new NotImplementedException();
                        }
                }
                currentOffset += currentInstruction.GetSize();
                currentIndex++;
            }

            return offsetToIndexMap;
        }

        private void ImportBasicBlocks(IDictionary<int, int> offsetToIndexMap)
        {
            _pendingBasicBlocks = _basicBlocks[0];
            while (_pendingBasicBlocks != null)
            {
                BasicBlock basicBlock = _pendingBasicBlocks;
                _pendingBasicBlocks = basicBlock.Next;

                ImportBasicBlock(offsetToIndexMap, basicBlock);
            }
        }

        private void ImportBasicBlock(IDictionary<int, int> offsetToIndexMap, BasicBlock basicBlock)
        {
            var currentOffset = basicBlock.StartOffset;
            var currentIndex = offsetToIndexMap[currentOffset];

            for (;;)
            {
                var currentInstruction = _method.Body.Instructions[currentIndex];
                currentOffset += currentInstruction.GetSize();
                currentIndex++;

                var opcode = currentInstruction.OpCode.Code;

                switch (opcode)
                {
                    case Code.Ldc_I4_0:
                    case Code.Ldc_I4_1:
                    case Code.Ldc_I4_2:
                    case Code.Ldc_I4_3:
                    case Code.Ldc_I4_4:
                    case Code.Ldc_I4_5:
                    case Code.Ldc_I4_6:
                    case Code.Ldc_I4_7:
                    case Code.Ldc_I4_8:
                        ImportLoadInt(opcode - Code.Ldc_I4_0);
                        break;

                    case Code.Ldc_I4_S:
                        ImportLoadInt((sbyte)currentInstruction.Operand);
                        break;

                    case Code.Add:
                        ImportAdd();
                        break;

                    case Code.Sub:
                        ImportSub();
                        break;

                    case Code.Ldarg_0:
                        break;

                    case Code.Stloc_0:
                        break;

                    case Code.Stloc_1:
                        break;

                    case Code.Ldloc_0:
                        break;

                    case Code.Ldloc_1:
                        break;

                    case Code.Ret:
                        ImportRet();
                        return;

                    case Code.Call:
                        ImportCall(currentInstruction.Operand as MethodDef);
                        break;

                    default:
                        _compilation.Logger.LogWarning($"Unsupport IL opcode {opcode}");
                        break;
                }

                if (currentOffset == _basicBlocks.Length)
                {
                    return;
                }

                var nextBasicBlock = _basicBlocks[currentOffset];
                if (nextBasicBlock != null)
                {
                    ImportFallThrough(nextBasicBlock);
                    return;
                }
            }
        }

        private void ImportFallThrough(BasicBlock next)
        {
            MarkBasicBlock(next);
        }

        private void MarkBasicBlock(BasicBlock basicBlock)
        {
            basicBlock.Next = _pendingBasicBlocks;
            _pendingBasicBlocks = basicBlock;
        }

        private void ImportAdd()
        {
            Append(Instruction.Pop(R16.HL));
            Append(Instruction.Pop(R16.DE));
            Append(Instruction.Add(R16.HL, R16.DE));
            Append(Instruction.Push(R16.HL));
        }

        private void ImportSub()
        {
            Append(Instruction.Pop(R16.HL));
            Append(Instruction.Pop(R16.DE));
            Append(Instruction.Sbc(R16.HL, R16.DE));
            Append(Instruction.Push(R16.HL));
        }

        private void ImportLdArg(short stackFrameSize)
        {
            var argumentOffset = stackFrameSize;
            argumentOffset += 2; // accounts for return address
            Append(Instruction.Ld(R8.H, I16.IX, (short)(argumentOffset + 1)));
            Append(Instruction.Ld(R8.L, I16.IX, argumentOffset));
            Append(Instruction.Push(R16.HL));
        }


        private void ImportRet()
        {
            Append(Instruction.Pop(R16.BC));
            Append(Instruction.Pop(R16.HL));
            Append(Instruction.Push(R16.BC));
            Append(Instruction.Push(R16.HL));

            Append(Instruction.Ret());
        }

        private void ImportCall(MethodDef methodToCall)
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

        private void ImportLoadInt(int value)
        {
            Append(Instruction.Ld(R16.HL, (short)value));
            Append(Instruction.Push(R16.HL));
        }

        private void Append(Instruction instruction)
        {
            _instructions.Add(instruction);
        }

        /*
        private void CreateStackFrame()
        {
            // Save IX
            _assembly.Push(I16.IX);

            // Use IX as frame pointer
            _assembly.Ld(I16.IX, 0);
            _assembly.Add(I16.IX, R16.SP);
        }
        */

        /* OLD CODE TO CALCULATE PARAMETER SIZE
         
                    short frameSize = 0;
            if (_method.Parameters.Count > 0)
            {
                foreach (var parameter in _method.Parameters)
                {
                    var type = parameter.Type;
                    if (type.IsCorLibType)
                    {
                        switch (type.TypeName)
                        {
                            case "Int16":
                                frameSize += 2;
                                break;
                            default:
                                break;
                        }
                    }
                }
                if (frameSize > 0)
                {
                    CreateStackFrame();
                }
            }         
         */
    }
}
