using System.Diagnostics;
using ILCompiler.Compiler.FlowgraphHelpers;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler
{
    public enum EHClauseKind
    {
        Typed = 0,  // Catch handler with a specific type
        Fault = 1,  // Fault handler
        Filter = 2,  // Filter handler
    }

    public class EHClause(BasicBlock tryBegin, BasicBlock tryLast, BasicBlock handlerBegin, BasicBlock handlerLast, BasicBlock? filterBegin,
        EHClauseKind kind, TypeDesc? exceptionType, uint tryBeginILOffset, uint tryEndILOffset)
    {
        public BasicBlock TryBegin { get; init; } = tryBegin;
        public BasicBlock TryLast { get; init; } = tryLast;
        public BasicBlock HandlerBegin { get; init; } = handlerBegin;
        public BasicBlock HandlerLast { get; init; } = handlerLast;
        public BasicBlock? FilterBegin { get; init; } = filterBegin;
        public EHClauseKind Kind { get; init; } = kind;
        public TypeDesc? ExceptionType { get; init;  } = exceptionType;

        public uint TryBeginILOffset { get; init; } = tryBeginILOffset;
        public uint TryEndILOffset { get; init; } = tryEndILOffset;
        public uint TrySize => TryEndILOffset - TryBeginILOffset;

        public bool InTry(uint il) => TryBeginILOffset <= il && il < TryEndILOffset;
    }

    public class BasicBlockAnalyser
    {
        private readonly MethodDesc _method;
        private readonly MethodIL _methodIL;
        private readonly IImporter? _importer;

        public int ReturnCount { get; set; }

        public BasicBlockAnalyser(MethodDesc method, MethodIL? methodIL = null, IImporter? importer = null)
        {
            _method = method;
            if (methodIL == null)
            {
                _methodIL = method.MethodIL!;
            }
            else
            {
                _methodIL = methodIL;
            }

            _importer = importer;
        }

        public BasicBlock[] FindBasicBlocks(IDictionary<uint, int> offsetToIndexMap, IList<EHClause> ehClauses)
        {
            var instructions = _methodIL.Instructions;
            var lastInstruction = instructions[instructions.Count - 1];
            var maxOffset = (int)lastInstruction.Offset + lastInstruction.GetSize();
            var basicBlocks = new BasicBlock[maxOffset];

            var jumpTargets = FindJumpTargets(offsetToIndexMap);
            MakeBasicBlocks(jumpTargets, basicBlocks);

            // Use a spill temp for the return value if there are multiple return blocks
            if (_importer is not null && _importer.Inlining)
            {
                if (_method.HasReturnType && ReturnCount > 1)
                {
                    // Create the spill temp
                    var returnType = _method.Signature.ReturnType;
                    var spillTempNumber = _importer.GrabTemp(returnType.VarType, returnType.GetElementSize().AsInt);
                    _importer.InlineInfo!.InlineeReturnSpillTempNumber = spillTempNumber;
                }
            }

            FindEHTargets(basicBlocks, ehClauses);

            return basicBlocks;
        }

        private void FindEHTargets(BasicBlock[] basicBlocks, IList<EHClause> ehClauses)
        {
            CreateEHBlocks(basicBlocks);
            CreateEHClauses(basicBlocks, ehClauses);
        }

        private void CreateEHBlocks(BasicBlock[] basicBlocks)
        {
            foreach (ILExceptionRegion exceptionHandler in _methodIL.GetExceptionRegions())
            {
                BasicBlock tryBeginBlock = CreateBasicBlock(basicBlocks, exceptionHandler.TryOffset);
                tryBeginBlock.EHFlags |= EHBoundaryFlags.TryStart;

                // Find previous block and set jump kind to always

                // If previous block when imported covered code prior to start of try
                // and code in the try upto a conditional then block will have jumpkind
                // of conditional but by splitting this into a new block starting at
                // the try offset then the original block needs to have the jump kind
                // set to Always as it now doesn't end with a conditional
                for (int i = (int)exceptionHandler.TryOffset - 1; i >= 0; i--)
                {
                    if (basicBlocks[i] != null)
                    {
                        basicBlocks[i].JumpKind = JumpKind.Always;
                        break;
                    }
                }

                BasicBlock? filterBlock;
                if (exceptionHandler.Kind == ILExceptionRegionKind.Filter)
                {
                    filterBlock = CreateBasicBlock(basicBlocks, (uint)exceptionHandler.FilterOffset!);
                    filterBlock.EHFlags |= EHBoundaryFlags.FilterStart;
                    tryBeginBlock.Handlers.Add(filterBlock);
                }

                BasicBlock handlerBeginBlock = CreateBasicBlock(basicBlocks, exceptionHandler.HandlerOffset);
                handlerBeginBlock.EHFlags |= EHBoundaryFlags.HandlerStart;
                if (exceptionHandler.Kind == ILExceptionRegionKind.Filter)
                {
                    handlerBeginBlock.CatchType = _method.Context.GetWellKnownType(WellKnownType.Object);
                }
                else
                {
                    handlerBeginBlock.CatchType = exceptionHandler.CatchType;
                }

                tryBeginBlock.Handlers.Add(handlerBeginBlock);
            }
        }

        private void CreateEHClauses(BasicBlock[] basicBlocks, IList<EHClause> ehClauses)
        {
            foreach (ILExceptionRegion exceptionHandler in _methodIL.GetExceptionRegions())
            {
                BasicBlock tryBeginBlock = basicBlocks[exceptionHandler.TryOffset];
                BasicBlock tryLastBlock = GetHandlerLastBlock(exceptionHandler.TryEndOffset, basicBlocks);

                BasicBlock handlerBeginBlock = basicBlocks[exceptionHandler.HandlerOffset];
                BasicBlock handlerLastBlock = GetHandlerLastBlock(exceptionHandler.HandlerEndOffset, basicBlocks);

                BasicBlock? filterBeginBlock = exceptionHandler.FilterOffset is not null ? basicBlocks[(uint)exceptionHandler.FilterOffset] : null;

                handlerBeginBlock.TryBlocks = GetTryBlocks(exceptionHandler, basicBlocks);

                TypeDesc? exceptionType = exceptionHandler.CatchType;

                uint methodEndILOffset = _methodIL.Instructions[^1].Offset;

                uint tryBeginILOffset = exceptionHandler.TryOffset;
                uint tryEndILOffset = exceptionHandler.TryEndOffset ?? methodEndILOffset;

                var ehClause = new EHClause(tryBeginBlock, tryLastBlock, handlerBeginBlock, handlerLastBlock, filterBeginBlock,
                    (EHClauseKind)exceptionHandler.Kind, exceptionType, tryBeginILOffset, tryEndILOffset);

                ehClauses.Add(ehClause);
            }
        }

        private static BasicBlock GetHandlerLastBlock(uint? handlerEndOffset, BasicBlock[] basicBlocks)
        {
            if (!handlerEndOffset.HasValue)
            {
                return basicBlocks[^1];
            }

            uint blockIndex = handlerEndOffset.Value - 1;
            BasicBlock? handlerLastBlock;
            do
            {
                handlerLastBlock = basicBlocks[blockIndex--];

            } while (handlerLastBlock is null && blockIndex >= 0);

            Debug.Assert(handlerLastBlock != null, "Could not find a basic block for the end of the handler");

            return handlerLastBlock;
        }

        private static List<BasicBlock> GetTryBlocks(ILExceptionRegion exceptionHandler, BasicBlock[] basicBlocks)
        {
            var tryStartOffset = exceptionHandler.TryOffset;
            var tryEndOffset = exceptionHandler.TryEndOffset ?? (uint)basicBlocks.Length;

            var tryBlocks = new List<BasicBlock>();
            for (uint offset = tryStartOffset; offset < tryEndOffset; offset++)
            {
                if (basicBlocks[offset] != null)
                {
                    tryBlocks.Add(basicBlocks[offset]);
                }
            }

            return tryBlocks;
        }

        private static BasicBlock CreateBasicBlock(BasicBlock[] basicBlocks, uint offset)
        {
            var basicBlock = basicBlocks[offset];
            if (basicBlock == null)
            {
                basicBlock = new BasicBlock(offset);                
                basicBlocks[offset] = basicBlock;
            }

            return basicBlock;
        }

        private void MakeBasicBlocks(List<uint> jumpTargets, BasicBlock[] basicBlocks)
        {
            var currentIndex = 0;
            uint currentOffset = 0;
            uint startOffset = 0;

            JumpKind? jumpKind = null;

            while (currentIndex < _methodIL.Instructions.Count)
            {
                uint? jumpAddress = null;
                var currentInstruction = _methodIL.Instructions[currentIndex];

                switch (currentInstruction.Opcode)
                {
                    case ILOpcode.blt_un:
                    case ILOpcode.ble_un:
                    case ILOpcode.bgt_un:
                    case ILOpcode.bge_un:
                    case ILOpcode.bne_un:
                    case ILOpcode.blt:
                    case ILOpcode.ble:
                    case ILOpcode.bgt:
                    case ILOpcode.bge:
                    case ILOpcode.beq:
                    case ILOpcode.brfalse:
                    case ILOpcode.brtrue:
                    case ILOpcode.blt_un_s:
                    case ILOpcode.ble_un_s:
                    case ILOpcode.bgt_un_s:
                    case ILOpcode.bge_un_s:
                    case ILOpcode.bne_un_s:
                    case ILOpcode.blt_s:
                    case ILOpcode.ble_s:
                    case ILOpcode.bgt_s:
                    case ILOpcode.bge_s:
                    case ILOpcode.beq_s:
                    case ILOpcode.brfalse_s:
                    case ILOpcode.brtrue_s:
                        jumpKind = JumpKind.Conditional;
                        jumpAddress = ((Instruction)currentInstruction.Operand).Offset;
                        break;

                    case ILOpcode.br_s:
                    case ILOpcode.leave_s:
                    case ILOpcode.br:
                    case ILOpcode.leave:
                        jumpKind = JumpKind.Always;
                        jumpAddress = ((Instruction)currentInstruction.Operand).Offset;
                        break;

                    case ILOpcode.throw_:
                    case ILOpcode.rethrow:
                        jumpKind = JumpKind.Always;
                        break;

                    case ILOpcode.switch_:
                        jumpKind = JumpKind.Switch;
                        break;

                    case ILOpcode.jmp:
                    case ILOpcode.ret:
                        jumpKind = JumpKind.Return;
                        break;

                    default:
                        break;
                }

                BasicBlock? currentBasicBlock = null;
                if (jumpKind is null)
                {
                    uint nextOffset = currentOffset + currentInstruction.GetSize();
                    var endOfBlock = jumpTargets.Contains(nextOffset);

                    if (endOfBlock)
                    {
                        currentBasicBlock = CreateBasicBlock(basicBlocks, startOffset);
                        currentBasicBlock.EndOffset = nextOffset;
                        currentBasicBlock.JumpKind = JumpKind.Always;

                        startOffset = nextOffset;
                    }
                }
                else
                {
                    var nextOffset = currentOffset + currentInstruction.GetSize();

                    currentBasicBlock = CreateBasicBlock(basicBlocks, startOffset);
                    currentBasicBlock.TargetOffset = jumpAddress;
                    currentBasicBlock.EndOffset = nextOffset;
                    currentBasicBlock.JumpKind = jumpKind.Value;

                    startOffset = nextOffset;
                }

                jumpKind = null;

                currentOffset += currentInstruction.GetSize();
                currentIndex++;
            }

            if (startOffset != currentOffset)
            {
                // Create the last block if it has not been created
                var basicBlock = CreateBasicBlock(basicBlocks, startOffset);
                basicBlock.EndOffset = currentOffset;
                basicBlock.JumpKind = JumpKind.Always;
            }

            LinkBasicBlocks(basicBlocks);
        }

        private static void LinkBasicBlocks(BasicBlock[] blocks)
        {
            foreach (BasicBlock block in blocks)
            {
                if (block is not null)
                {
                    switch (block.JumpKind)
                    {
                        case JumpKind.Conditional:
                            Debug.Assert(block.TargetOffset is not null);
                            BasicBlock trueTarget = blocks[block.TargetOffset.Value];
                            FlowEdge trueEdge = new FlowEdge(block, trueTarget);
                            block.TrueEdge = trueEdge;

                            break;
                    }
                }
            }
        }

        private List<uint> FindJumpTargets(IDictionary<uint, int> offsetToIndexMap)
        {
            var jumpTargets = new List<uint>();

            var currentIndex = 0;
            uint currentOffset = 0;

            while (currentIndex < _methodIL.Instructions.Count)
            {
                offsetToIndexMap[currentOffset] = currentIndex;
                var currentInstruction = _methodIL.Instructions[currentIndex];
                
                switch (currentInstruction.Opcode)
                {
                    case ILOpcode.blt_un:
                    case ILOpcode.ble_un:
                    case ILOpcode.bgt_un:
                    case ILOpcode.bge_un:
                    case ILOpcode.bne_un:
                    case ILOpcode.blt:
                    case ILOpcode.ble:
                    case ILOpcode.bgt:
                    case ILOpcode.bge:
                    case ILOpcode.beq:
                    case ILOpcode.brfalse:
                    case ILOpcode.brtrue:
                    case ILOpcode.blt_un_s:
                    case ILOpcode.ble_un_s:
                    case ILOpcode.bgt_un_s:
                    case ILOpcode.bge_un_s:
                    case ILOpcode.bne_un_s:
                    case ILOpcode.blt_s:
                    case ILOpcode.ble_s:
                    case ILOpcode.bgt_s:
                    case ILOpcode.bge_s:
                    case ILOpcode.beq_s:
                    case ILOpcode.brfalse_s:
                    case ILOpcode.brtrue_s:
                        {
                            var target = (Instruction)currentInstruction.Operand;
                            var targetOffset = target.Offset;
                            jumpTargets.Add(targetOffset);
                            uint nextInstructionOffset = currentOffset + currentInstruction.GetSize();
                            jumpTargets.Add(nextInstructionOffset);
                        }
                        break;

                    case ILOpcode.br_s:
                    case ILOpcode.leave_s:
                    case ILOpcode.br:
                    case ILOpcode.leave:
                        {
                            var target = (Instruction)currentInstruction.Operand;
                            jumpTargets.Add(target.Offset);
                        }
                        break;

                    case ILOpcode.ret:
                        {
                            ReturnCount++;
                        }
                        break;

                    case ILOpcode.switch_:
                        {
                            var instructions = (Instruction[])currentInstruction.Operand;
                            foreach (var target in instructions)
                            {
                                jumpTargets.Add(target.Offset);
                            }
                            var nextInstructionOffset = currentOffset + currentInstruction.GetSize();
                            jumpTargets.Add(nextInstructionOffset);
                        }
                        break;
                    case ILOpcode.ldarga:
                    case ILOpcode.ldarga_s:
                        {
                            var parameter = (ParameterDefinition)currentInstruction.Operand;
                            var index = parameter.Index;

                            if (_importer!.Inlining)
                            {
                                _importer.InlineInfo!.InlineArgumentInfos[index].HasLdargaOp = true;
                            }
                        }
                        break;

                    case ILOpcode.starg:
                    case ILOpcode.starg_s:
                        {
                            var parameter = (ParameterDefinition)currentInstruction.Operand;
                            var index = parameter.Index;

                            if (_importer!.Inlining)
                            {
                                _importer.InlineInfo!.InlineArgumentInfos[index].HasStargOp = true;
                            }
                        }
                        break;

                }
                currentOffset += currentInstruction.GetSize();
                currentIndex++;
            }

            return jumpTargets;
        }
    }
}
