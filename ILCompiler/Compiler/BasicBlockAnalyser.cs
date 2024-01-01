using dnlib.DotNet.Emit;
using ILCompiler.Common.TypeSystem.Common;

namespace ILCompiler.Compiler
{
    public class BasicBlockAnalyser
    {
        private readonly MethodDesc _method;

        public BasicBlockAnalyser(MethodDesc method)
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

            FindEHTargets(basicBlocks);

            return basicBlocks;
        }

        private void FindEHTargets(BasicBlock[] basicBlocks)
        {
            foreach (var exceptionHandler in _method.Body.ExceptionHandlers)
            {
                CreateBasicBlock(basicBlocks, (int)exceptionHandler.TryStart.Offset).TryStart = true;
                if (exceptionHandler.IsFilter)
                {
                    CreateBasicBlock(basicBlocks, (int)exceptionHandler.FilterStart.Offset).FilterStart = true;
                }
                CreateBasicBlock(basicBlocks, (int)exceptionHandler.HandlerStart.Offset).HandlerStart = true;
            }
        }

        private static BasicBlock CreateBasicBlock(BasicBlock[] basicBlocks, int offset)
        {
            var basicBlock = basicBlocks[offset];
            if (basicBlock == null)
            {
                basicBlock = new BasicBlock(offset);
                basicBlocks[offset] = basicBlock;
            }

            return basicBlock;
        }

        private void FindJumpTargets(BasicBlock[] basicBlocks, IDictionary<int, int> offsetToIndexMap)
        {
            var currentIndex = 0;
            var currentOffset = 0;

            var currentBlock = basicBlocks[0];

            while (currentIndex < _method.Body.Instructions.Count)
            {
                offsetToIndexMap[currentOffset] = currentIndex;
                var currentInstruction = _method.Body.Instructions[currentIndex];

                if (basicBlocks[currentOffset] != null)
                {
                    currentBlock.EndOffset = currentOffset;
                    currentBlock = basicBlocks[currentOffset];
                }
                
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
                            currentBlock.JumpKind = JumpKind.Conditional;
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
                            currentBlock.JumpKind = JumpKind.Always;
                            var target = currentInstruction.OperandAs<Instruction>();
                            CreateBasicBlock(basicBlocks, (int)target.Offset); // target of jump
                        }
                        break;

                    case Code.Ret:
                        {
                            currentBlock.JumpKind = JumpKind.Return;
                        }
                        break;

                    case Code.Switch:
                        {
                            currentBlock.JumpKind = JumpKind.Switch;
                            if (currentInstruction.Operand is Instruction[] targets)
                            {
                                foreach (var target in targets)
                                {
                                    CreateBasicBlock(basicBlocks, (int)target.Offset); // target of jump
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

            currentBlock.EndOffset = currentOffset;
        }
    }
}
