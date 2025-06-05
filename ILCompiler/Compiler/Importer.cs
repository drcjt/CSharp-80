using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.OpcodeImporters;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;
using Microsoft.Extensions.Logging;
using PreinitializationManager = ILCompiler.Compiler.PreInit.PreinitializationManager;
namespace ILCompiler.Compiler
{
    public class Importer : IImporter
    {
        public IConfiguration Configuration { get; init; }
        private ILogger<Importer> Logger { get; init; }
        public INameMangler NameMangler { get; init; }
        public PreinitializationManager PreinitializationManager { get; init; }
        public NodeFactory NodeFactory { get; init; }

        public MethodDesc Method { get; private set; } = null!;
        private MethodIL _methodIL = null!;

        private LocalVariableTable? _locals;
        public LocalVariableTable LocalVariableTable => _locals!;

        public BasicBlock[] BasicBlocks { get; set; } = [];
        private BasicBlock? _currentBasicBlock;
        private BasicBlock? _pendingBasicBlocks;
        public BasicBlock? FallThroughBlock { get; private set; } = null;

        public int ParameterCount { get; private set; }
        public int? ReturnBufferArgIndex { get; private set; }

        private readonly EvaluationStack<StackEntry> _stack = new EvaluationStack<StackEntry>(0);
        public void Push(StackEntry entry) => _stack.Push(entry);
        public StackEntry Pop() => _stack.Pop();

        private readonly IEnumerable<IOpcodeImporter> _opcodeImporters;

        public InlineInfo? InlineInfo { get; private set; }
        public bool Inlining => InlineInfo != null;

        public TypeDesc? Constrained { get; set; } = null;

        public bool StopImporting { get; set; }

        public Importer(IConfiguration configuration, ILogger<Importer> logger, INameMangler nameMangler, IEnumerable<IOpcodeImporter> importers, PreinitializationManager preinitializationManager, NodeFactory nodeFactory)
        {
            Configuration = configuration;
            NameMangler = nameMangler;
            PreinitializationManager = preinitializationManager;
            NodeFactory = nodeFactory;
            Logger = logger;
            _opcodeImporters = importers;
        }

        private void ImportBasicBlocks(IDictionary<int, int> offsetToIndexMap)
        {
            _pendingBasicBlocks = BasicBlocks[0];
            _pendingBasicBlocks.Marked = true;
            while (_pendingBasicBlocks != null)
            {
                BasicBlock basicBlock = _pendingBasicBlocks;
                _pendingBasicBlocks = basicBlock.Next;

                StartImportingBasicBlock(basicBlock);
                ImportBasicBlock(offsetToIndexMap, basicBlock);
                EndImportingBasicBlock(basicBlock);
            }
        }

        private void StartImportingBasicBlock(BasicBlock basicBlock)
        {
            _stack.Clear();

            if (basicBlock.EntryStack != null)
            {
                EvaluationStack<StackEntry> entryStack = basicBlock.EntryStack;
                int n = entryStack.Length;
                for (int i = 0; i < n; i++)
                {
                    _stack.Push(entryStack[i].Duplicate());
                }
            }

            // Add exception object for catch
            if (basicBlock.HandlerStart)
            {
                // Use a node type that won't generate any code
                _stack.Push(new CatchArgumentEntry());
            }

            foreach (var handlerBlock in basicBlock.Handlers)
            {
                MarkBasicBlock(handlerBlock);
            }
        }

        private static void EndImportingBasicBlock(BasicBlock basicBlock)
        {
            // TODO: add any appropriate code to handle the end of importing a basic block
        }

        private void ImportSpillAppendTree(StackEntry entry)
        {
            // Spill any existing stack entries to temps to preserve evaluation order
            ImportSpillStackEntries();
            ImportAppendTree(entry);
        }

        private void ImportSpillStackEntries()
        {
            for (int i = 0; i < _stack.Length; i++)
            {
                // TODO: Check if this evaluation stack entry refers to the local to spill
                // if not then it can be skipped otherwise ...

                // Create an assignment node from the spilled stack entry to a new temp
                // The return value is a local var node for the new temp
                var tempLocalVar = ImportSpillStackEntry(_stack[i], null);

                // Swap out the existing local var reference for the new local var node for the temp
                _stack[i] = tempLocalVar;
            }
        }

        public void ImportAppendTree(StackEntry entry, bool spill = false)
        {
            if (spill)
            {
                ImportSpillAppendTree(entry);
            }
            else
            {                
                _currentBasicBlock?.Statements.Add(new Statement(entry));
            }
        }

        private StackEntry? ImportExtractLasStatement()
        {
            if (_currentBasicBlock?.Statements.Count > 0)
            {
                var lastStatementIndex = _currentBasicBlock.Statements.Count - 1;
                var lastStatement = _currentBasicBlock.Statements[lastStatementIndex];
                var lastStatementRoot = lastStatement.RootNode;

                if (lastStatementRoot is JumpEntry || lastStatementRoot is JumpTrueEntry)
                {
                    // Remove the branch statement from the current basic block
                    _currentBasicBlock.Statements.RemoveAt(lastStatementIndex);
                    return lastStatementRoot;
                }
            }

            return null;
        }

        private void ImportBasicBlock(IDictionary<int, int> offsetToIndexMap, BasicBlock block)
        {
            _currentBasicBlock = block;
            var currentOffset = block.StartOffset;
            var currentIndex = offsetToIndexMap[currentOffset];

            StopImporting = false;

            while (true)
            {
                var currentInstruction = _methodIL!.Instructions[currentIndex];
                currentOffset += currentInstruction.GetSize();
                currentIndex++;

                FallThroughBlock = currentOffset < BasicBlocks.Length ? BasicBlocks[currentOffset] : null;

                bool imported = ImportInstruction(currentInstruction);

                if (imported)
                {
                    if (StopImporting)
                    {
                        return;
                    }
                }
                else
                {
                    HandleUnknownInstruction(currentInstruction);
                }

                if (currentOffset == BasicBlocks.Length)
                {
                    return;
                }

                var nextBasicBlock = BasicBlocks[currentOffset];
                if (nextBasicBlock != null)
                {
                    ImportFallThrough(nextBasicBlock);
                    return;
                }
            }
        }

        private void HandleUnknownInstruction(Instruction currentInstruction)
        {
            if (Configuration.IgnoreUnknownCil)
            {
                Logger.LogWarning("Unsupported IL opcode {Opcode} in {MethodFullName}", currentInstruction.Opcode, Method.FullName);
                return;
            }

            throw new UnknownCilException($"Unsupported IL opcode {currentInstruction.Opcode} in {Method.FullName}");
        }

        private bool ImportInstruction(Instruction currentInstruction)
        {
            var imported = false;
            foreach (var opcodeImporter in _opcodeImporters)
            {
                if (opcodeImporter.Import(currentInstruction, this))
                {
                    imported = true;
                    break;
                }
            }

            return imported;
        }

        private LocalVariableEntry ImportSpillStackEntry(StackEntry entry, int? tempNumber = null)
        {
            tempNumber = tempNumber ?? _locals!.GrabTemp(entry.Type, entry.ExactSize);

            var node = new StoreLocalVariableEntry(tempNumber.Value, false, entry.Duplicate());
            ImportAppendTree(node);

            return new LocalVariableEntry(tempNumber.Value, entry.Type, entry.ExactSize);
        }

        private void MarkBasicBlock(BasicBlock basicBlock)
        {
            if (!basicBlock.Marked)
            {
                basicBlock.Next = _pendingBasicBlocks;
                _pendingBasicBlocks = basicBlock;
                basicBlock.Marked = true;
            }
        }

        public IList<BasicBlock> Import(int parameterCount, int? returnBufferArgIndex, MethodDesc method, LocalVariableTable locals, IList<EHClause> ehClauses, InlineInfo? inlineInfo = null)
        {
            InlineInfo = inlineInfo;

            ParameterCount = parameterCount;
            ReturnBufferArgIndex = returnBufferArgIndex;

            Method = method;
            _locals = locals;

            var methodIL = method.MethodIL!;

            var uninstantiatedMethodIL = methodIL.GetMethodILDefinition();
            if (methodIL != uninstantiatedMethodIL)
            {
                var sharedMethod = Method.GetSharedRuntimeFormMethodTarget();
                _methodIL = new InstantiatedMethodIL(sharedMethod, uninstantiatedMethodIL);
            }
            else
            {
                _methodIL = methodIL;
            }

            var basicBlockAnalyser = new BasicBlockAnalyser(Method, NameMangler, _methodIL);
            var offsetToIndexMap = new Dictionary<int, int>();
            BasicBlocks = basicBlockAnalyser.FindBasicBlocks(offsetToIndexMap, ehClauses);

            // Trigger static constructor if required
            if (method.IsDefaultConstructor)
            {
                var staticConstructorMethod = method.OwningType.GetStaticConstructor();
                if (staticConstructorMethod != null)
                {
                    var targetMethod = NameMangler.GetMangledMethodName(staticConstructorMethod);
                    var staticInitCall = new CallEntry(targetMethod, [], VarType.Void, 0);

                    BasicBlocks[0].Statements.Add(new Statement(staticInitCall));
                }
            }

            ImportBasicBlocks(offsetToIndexMap);

            var importedBasicBlocks = new List<BasicBlock>();
            for (int i = 0; i < BasicBlocks.Length; i++)
            {
                if (BasicBlocks[i] != null)
                {                    
                    importedBasicBlocks.Add(BasicBlocks[i]);
                }
            }

            // Add EH begin block as successor to all blocks in trys
            SetTryBlockSuccessors(importedBasicBlocks);

            return importedBasicBlocks;
        }

        private static void SetTryBlockSuccessors(IList<BasicBlock> basicBlocks)
        {
            foreach (var block in basicBlocks)
            {
                if (block.HandlerStart)
                {
                    foreach (var tryBlock in block.TryBlocks)
                    {
                        tryBlock.Successors.Add(block);
                    }
                }
            }
        }

        public void ImportFallThrough(BasicBlock next)
        {
            // Setup successor of current block as next
            _currentBasicBlock!.Successors.Add(next);

            // Setup predecessor of next block as current block
            next.Predecessors.Add(_currentBasicBlock);

            // Evaluation stack in each basic block holds the imported high level tree representation of the IL

            EvaluationStack<StackEntry>? entryStack = next.EntryStack;

            if (entryStack != null)
            {
                // III.1.8.1.3 Merging Stack States

                // The number of slots in each stack state must be the same
                if (entryStack.Length != _stack.Length)
                {
                    throw new InvalidProgramException();
                }

                //  ECMA Common Language Infrastructure(CLI) standard, V6, Partition III, CIL Instruction Set
                // in section III.1.8.1.3 "Merging stack states", describes how stack states should be merged.
                // For the case where the types are assignable either way (e.g. as for int32 and native int),
                // the merging must keep the first type found.
                for (int i = 0; i < entryStack.Length; i++)
                {
                    // TODO: Use Compatibility function from ECMA standard here
                    if (entryStack[i].Type != _stack[i].Type)
                    {
                        //throw new InvalidProgramException();
                    }
                }
            }

            if (_stack.Length > 0)
            {
                // If the block is not empty on exit from the basic block, additional
                // processing is needed to make sure the block successors can use the
                // values on the predecessor’s stack.
                // 
                // Any branch/jump statement is removed.
                // Next the remaining values on the stack are removed and converted
                // into assignments to new temps
                // The next basic blocks entry stack is setup to contain the temps
                // The branch/jump statement is put back if there was one

                var setupEntryStack = entryStack == null;
                if (entryStack == null)
                {
                    entryStack = new EvaluationStack<StackEntry>(_stack.Length);
                }

                // Remove the branch/jump statement at the end of the block if present
                var lastStatement = ImportExtractLasStatement();

                // Spill stuff on stack in new temps and setup new stack to contain the temps
                for (int i = 0; i < _stack.Length; i++)
                {
                    int? tempNumber = null;
                    if (!setupEntryStack)
                    {
                        tempNumber = (entryStack[i].As<LocalVariableEntry>()).LocalNumber;
                    }
                    var temp = ImportSpillStackEntry(_stack[i], tempNumber);
                    if (setupEntryStack)
                    {
                        entryStack.Push(temp);
                    }
                }

                // Add the branch/jump statement back if there was one
                if (lastStatement != null)
                {
                    ImportAppendTree(lastStatement);
                }

                // Set the entry stack for the next basic block to the one we've built
                // e.g. with the new temps on it
                next.EntryStack = entryStack;
            }

            MarkBasicBlock(next);
        }

        public StackEntry InlineFetchArgument(int ilArgNum)
        {
            var inlineArgumentInfo = InlineInfo!.InlineArgumentInfos[ilArgNum];

            if (inlineArgumentInfo.HasTemp)
            {
                var callArgument = inlineArgumentInfo.Argument;
                var tempNumber = inlineArgumentInfo.TempNumber;
                return new LocalVariableEntry(tempNumber, callArgument.Type, callArgument.ExactSize);
            }
            else
            {
                var callArgument = inlineArgumentInfo.Argument;

                var tempNumber = GrabTemp(callArgument.Type, callArgument.ExactSize);

                inlineArgumentInfo.HasTemp = true;
                inlineArgumentInfo.TempNumber = tempNumber;

                return new LocalVariableEntry(tempNumber, callArgument.Type, callArgument.ExactSize);
            }
        }

        public int InlineFetchLocal(int localNumber)
        {
            var inlineLocalInfo = InlineInfo!.LocalVariableInfos[localNumber];
            if (inlineLocalInfo.HasTemp)
            {
                return inlineLocalInfo.TempNumber;
            }
            else
            {
                var localType = inlineLocalInfo.Type;

                var tempNumber = GrabTemp(localType, localType.GetTypeSize());

                inlineLocalInfo.HasTemp = true;
                inlineLocalInfo.TempNumber = tempNumber;

                return tempNumber;
            }
        }

        public int GrabTemp(VarType type, int? exactSize)
        {
            if (Inlining)
            {
                // Grab temp in locals of method containing call that is being inlined
                return InlineInfo!.InlineLocalVariableTable.GrabTemp(type, exactSize);
            }
            else
            {
                return _locals!.GrabTemp(type, exactSize);
            }
        }

        public StackEntry GetGenericContext()
        {
            if (Method.AcquiresInstMethodTableFromThis())
            {
                var thisPtr = new LocalVariableEntry(0, VarType.Ref, 2);
                var thisType = new IndirectEntry(thisPtr, VarType.Ptr, 2);

                return thisType;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}