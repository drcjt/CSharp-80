using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
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
        private readonly MethodDesc _method;
        private readonly MethodIL _methodIL;
        private readonly IImporter? _importer;

        public int ReturnCount { get; set; }

        public BasicBlockAnalyser(MethodDesc method, MethodIL? methodIL = null) : this(method, new NameMangler(), methodIL)
        {
        }

        public BasicBlockAnalyser(MethodDesc method, INameMangler nameMangler, MethodIL? methodIL = null, IImporter? importer = null)
        {
            _method = method;
            _nameMangler = nameMangler;
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

        public BasicBlock[] FindBasicBlocks(IDictionary<int, int> offsetToIndexMap, IList<EHClause> ehClauses)
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
            foreach (var exceptionHandler in _methodIL.GetExceptionRegions())
            {
                var tryBeginBlock = CreateBasicBlock(basicBlocks, exceptionHandler.TryOffset);

                // Find previous block and set jump kind to always

                // If previous block when imported covered code prior to start of try
                // and code in the try upto a conditional then block will have jumpkind
                // of conditional but by splitting this into a new block starting at
                // the try offset then the original block needs to have the jump kind
                // set to Always as it now doesn't end with a conditional
                for (int i = exceptionHandler.TryOffset - 1; i >= 0; i--)
                {
                    if (basicBlocks[i] != null)
                    {
                        basicBlocks[i].JumpKind = JumpKind.Always;
                        break;
                    }
                }

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

        private void MakeBasicBlocks(List<int> jumpTargets, BasicBlock[] basicBlocks)
        {
            var currentIndex = 0;
            var currentOffset = 0;
            var startOffset = 0;

            JumpKind? jumpKind = null;

            while (currentIndex < _methodIL.Instructions.Count)
            {
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
                        break;

                    case ILOpcode.br_s:
                    case ILOpcode.leave_s:
                    case ILOpcode.br:
                    case ILOpcode.leave:
                        jumpKind = JumpKind.Always;
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

                if (jumpKind is null)
                {
                    var nextOffset = currentOffset + currentInstruction.GetSize();
                    var endOfBlock = jumpTargets.Contains(nextOffset);

                    if (endOfBlock)
                    {
                        var basicBlock = CreateBasicBlock(basicBlocks, startOffset);
                        basicBlock.EndOffset = nextOffset;
                        basicBlock.JumpKind = JumpKind.Always;

                        startOffset = nextOffset;
                    }
                }
                else
                {
                    var nextOffset = currentOffset + currentInstruction.GetSize();

                    var basicBlock = CreateBasicBlock(basicBlocks, startOffset);
                    basicBlock.EndOffset = nextOffset;
                    basicBlock.JumpKind = jumpKind.Value;

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
        }

        private List<int> FindJumpTargets(IDictionary<int, int> offsetToIndexMap)
        {
            var jumpTargets = new List<int>();

            var currentIndex = 0;
            var currentOffset = 0;

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
                            jumpTargets.Add((int)targetOffset);
                            var nextInstructionOffset = currentOffset + currentInstruction.GetSize();
                            jumpTargets.Add(nextInstructionOffset);
                        }
                        break;

                    case ILOpcode.br_s:
                    case ILOpcode.leave_s:
                    case ILOpcode.br:
                    case ILOpcode.leave:
                        {
                            var target = (Instruction)currentInstruction.Operand;
                            jumpTargets.Add((int)target.Offset);
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
                                jumpTargets.Add((int)target.Offset);
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
