using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ILCompiler.Interfaces;
using System;
using System.Collections.Generic;

namespace ILCompiler.Compiler
{
    public class BasicBlockAnalyser
    {
        private readonly IILImporter _importer;
        private readonly MethodDef _method;
        private BasicBlock[] _basicBlocks;

        public BasicBlock[] BasicBlocks
        {
            get
            {
                return _basicBlocks;
            }
        }

        public BasicBlockAnalyser(MethodDef method, IILImporter importer)
        {
            _importer = importer;
            _method = method;
        }

        public IDictionary<int, int> FindBasicBlocks()
        {
            var instructions = _method.Body.Instructions;
            var lastInstruction = instructions[instructions.Count - 1];
            var maxOffset = (int)lastInstruction.Offset + lastInstruction.GetSize();
            _basicBlocks = new BasicBlock[maxOffset];

            CreateBasicBlock(0);

            return FindJumpTargets();
        }

        private void CreateBasicBlock(int offset)
        {
            var basicBlock = _basicBlocks[offset];
            if (basicBlock == null)
            {
                basicBlock = new BasicBlock(_importer, offset);
                _basicBlocks[offset] = basicBlock;
            }
        }

        private IDictionary<int, int> FindJumpTargets()
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
    }
}
