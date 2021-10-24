using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;

namespace ILCompiler.Compiler
{
    public class BasicBlockAnalyser
    {
        private readonly MethodDef _method;

        public BasicBlockAnalyser(MethodDef method)
        {
            _method = method;
        }

        public BasicBlock[] FindBasicBlocks(IDictionary<int, int> offsetToIndexMap)
        {
            var instructions = _method.Body.Instructions;
            var lastInstruction = instructions[instructions.Count - 1];
            var maxOffset = (int)lastInstruction.Offset + lastInstruction.GetSize();
            var basicBlocks = new BasicBlock[maxOffset];

            CreateBasicBlock(basicBlocks, 0);

            FindJumpTargets(basicBlocks, offsetToIndexMap);

            return basicBlocks;
        }

        private void CreateBasicBlock(BasicBlock[] basicBlocks,  int offset)
        {
            var basicBlock = basicBlocks[offset];
            if (basicBlock == null)
            {
                basicBlock = new BasicBlock(offset);
                basicBlocks[offset] = basicBlock;
            }
        }

        private void FindJumpTargets(BasicBlock[] basicBlocks, IDictionary<int, int> offsetToIndexMap)
        {
            var currentIndex = 0;
            var currentOffset = 0;

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
                            var target = currentInstruction.OperandAs<Instruction>();
                            var targetOffset = target.Offset;
                            CreateBasicBlock(basicBlocks, (int)targetOffset); // target of jump                            
                            var nextInstructionOffset = currentOffset + currentInstruction.GetSize();
                            CreateBasicBlock(basicBlocks, nextInstructionOffset); // instruction after jump
                        }
                        break;

                    case Code.Br_S:
                    case Code.Leave_S:
                    case Code.Br:
                    case Code.Leave:
                        {
                            var target = currentInstruction.OperandAs<Instruction>();
                            CreateBasicBlock(basicBlocks,  (int)target.Offset); // target of jump
                        }
                        break;

                    case Code.Switch:
                        {
                            var targets = currentInstruction.Operand as dnlib.DotNet.Emit.Instruction[];
                            if (targets != null)
                            {
                                foreach (var target in targets)
                                {
                                    var targetOffset = target.Offset;
                                    CreateBasicBlock(basicBlocks,  (int)targetOffset); // target of jump
                                }
                            }
                            var nextInstructionOffset = currentOffset + currentInstruction.GetSize();
                            CreateBasicBlock(basicBlocks, nextInstructionOffset); // instruction after jump
                        }
                        break;
                }
                currentOffset += currentInstruction.GetSize();
                currentIndex++;
            }
        }
    }
}
