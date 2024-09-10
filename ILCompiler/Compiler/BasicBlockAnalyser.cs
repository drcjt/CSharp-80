using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.Dnlib;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler
{
    public enum EHClauseKind
    {
        Typed,
        Fault,
        Filter
    }

    public class EHClause
    {
        public BasicBlock TryBegin { get; init; }
        public BasicBlock? TryEnd { get; init; }
        public BasicBlock HandlerBegin { get; init; }
        public BasicBlock? HandlerEnd { get; init; }
        public BasicBlock? Filter { get; init; }
        public EHClauseKind Kind { get; init; }
        public string CatchTypeMangledName { get; init; }

        public EHClause(BasicBlock tryBegin, BasicBlock? tryEnd, BasicBlock handlerBegin, BasicBlock? handlerEnd, BasicBlock? filter, EHClauseKind kind, string catchTypeMangledName)
        {
            TryBegin = tryBegin;
            TryEnd = tryEnd;
            HandlerBegin = handlerBegin;
            HandlerEnd = handlerEnd;
            Filter = filter;
            Kind = kind;
            CatchTypeMangledName = catchTypeMangledName;
        }   
    }

    public class BasicBlockAnalyser
    {
        private readonly INameMangler _nameMangler;
        private readonly MethodIL _methodIL;

        public BasicBlockAnalyser(MethodDesc method, MethodIL? methodIL = null) : this(method, new NameMangler(), methodIL)
        {
        }

        public BasicBlockAnalyser(MethodDesc method, INameMangler nameMangler, MethodIL? methodIL = null)
        {
            _nameMangler = nameMangler;
            if (methodIL == null)
            {
                _methodIL = method.MethodIL!;
            }
            else
            {
                _methodIL = methodIL;
            }
        }

        public BasicBlock[] FindBasicBlocks(IDictionary<int, int> offsetToIndexMap, IList<EHClause> ehClauses)
        {
            var instructions = _methodIL.Instructions;
            var lastInstruction = instructions[instructions.Count - 1];
            var maxOffset = (int)lastInstruction.Offset + lastInstruction.GetSize();
            var basicBlocks = new BasicBlock[maxOffset];

            CreateBasicBlock(basicBlocks, 0);

            FindJumpTargets(basicBlocks, offsetToIndexMap);
            FindEHTargets(basicBlocks, ehClauses);

            return basicBlocks;
        }

        private void FindEHTargets(BasicBlock[] basicBlocks, IList<EHClause> ehClauses)
        {
            foreach (var exceptionHandler in _methodIL.GetExceptionRegions())
            {
                var tryBeginBlock = CreateBasicBlock(basicBlocks, exceptionHandler.TryOffset);
                BasicBlock? filterBlock = null;

                if (exceptionHandler.Kind != ILExceptionRegionKind.Catch)
                {
                    // TODO: Implement finally blocks - just ignore them for now
                    continue;
                }

                if (exceptionHandler.Kind == ILExceptionRegionKind.Filter)
                {
                    filterBlock = CreateBasicBlock(basicBlocks, exceptionHandler.FilterOffset);
                    filterBlock.FilterStart = true;
                }

                var handlerBeginBlock = CreateBasicBlock(basicBlocks, exceptionHandler.HandlerOffset);
                var handlerEndBlock = exceptionHandler.HandlerEndOffset != null ? basicBlocks[(int)exceptionHandler.HandlerEndOffset] : null;

                handlerBeginBlock.TryBlocks = GetTryBlocks(exceptionHandler, basicBlocks);

                var tryEndBlock = exceptionHandler.TryEndOffset != null ? basicBlocks[(int)exceptionHandler.TryEndOffset] : null;

                handlerBeginBlock.HandlerStart = true;
                tryBeginBlock.TryStart = true;

                var catchTypeDesc = exceptionHandler.CatchType!;
                var catchTypeMangledName = _nameMangler.GetMangledTypeName(catchTypeDesc);

                tryBeginBlock.Handlers.Add(handlerBeginBlock);

                EHClauseKind kind = EHClauseKind.Typed;
                if (exceptionHandler.Kind == ILExceptionRegionKind.Fault || exceptionHandler.Kind == ILExceptionRegionKind.Finally) kind = EHClauseKind.Fault;
                else if (exceptionHandler.Kind == ILExceptionRegionKind.Filter) kind = EHClauseKind.Filter;

                var ehClause = new EHClause(tryBeginBlock, tryEndBlock, handlerBeginBlock, handlerEndBlock, filterBlock, kind, catchTypeMangledName);
                ehClauses.Add(ehClause);
            }
        }

        private static List<BasicBlock> GetTryBlocks(ILExceptionRegion exceptionHandler, BasicBlock[] basicBlocks)
        {
            var tryStartOffset = exceptionHandler.TryOffset;
            var tryEndOffset = exceptionHandler.TryEndOffset ?? basicBlocks.Length;

            var tryBlocks = new List<BasicBlock>();
            for (int offset = tryStartOffset; offset < tryEndOffset; offset++)
            {
                if (basicBlocks[offset] != null)
                {
                    tryBlocks.Add(basicBlocks[offset]);
                }
            }

            return tryBlocks;
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

            while (currentIndex < _methodIL.Instructions.Count)
            {
                offsetToIndexMap[currentOffset] = currentIndex;
                var currentInstruction = _methodIL.Instructions[currentIndex];

                if (basicBlocks[currentOffset] != null)
                {
                    currentBlock.EndOffset = currentOffset;
                    currentBlock = basicBlocks[currentOffset];
                }
                
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
                            currentBlock.JumpKind = JumpKind.Conditional;
                            var target = (Instruction)currentInstruction.GetOperand();
                            var targetOffset = target.Offset;
                            CreateBasicBlock(basicBlocks, (int)targetOffset); // target of jump                            
                            var nextInstructionOffset = currentOffset + currentInstruction.GetSize();
                            CreateBasicBlock(basicBlocks, nextInstructionOffset); // instruction after jump
                        }
                        break;

                    case ILOpcode.br_s:
                    case ILOpcode.leave_s:
                    case ILOpcode.br:
                    case ILOpcode.leave:
                        {
                            currentBlock.JumpKind = JumpKind.Always;
                            var target = (Instruction)currentInstruction.GetOperand();
                            CreateBasicBlock(basicBlocks, (int)target.Offset); // target of jump
                        }
                        break;

                    case ILOpcode.ret:
                        {
                            currentBlock.JumpKind = JumpKind.Return;
                        }
                        break;

                    case ILOpcode.switch_:
                        {
                            currentBlock.JumpKind = JumpKind.Switch;
                            if (currentInstruction.OperandIsNotNull)
                            {
                                var instructions = (Instruction[])currentInstruction.GetOperand();
                                foreach (var target in instructions)
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