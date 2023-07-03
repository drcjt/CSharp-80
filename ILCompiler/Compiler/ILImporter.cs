using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.Importer;
using ILCompiler.Interfaces;
using Microsoft.Extensions.Logging;

namespace ILCompiler.Compiler
{
    public class ILImporter : IILImporter
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ILImporter> _logger;
        private readonly INameMangler _nameMangler;
        private readonly CorLibModuleProvider _corLibModuleProvider;

        private MethodDesc _method = null!;
        private IList<LocalVariableDescriptor> _localVariableTable = null!;

        private BasicBlock[] _basicBlocks;
        private BasicBlock? _currentBasicBlock;
        private BasicBlock? _pendingBasicBlocks;

        private int _parameterCount;
        private int? _returnBufferArgIndex;

        private readonly EvaluationStack<StackEntry> _stack = new EvaluationStack<StackEntry>(0);

        private readonly IEnumerable<IOpcodeImporter> _importers;
        private readonly ILImporterProxy _importerProxy;

        private class ILImporterProxy : IILImporterProxy
        {
            private readonly ILImporter _importer;
            public ILImporterProxy(ILImporter importer)
            {
                _importer = importer;
            }

            public int ParameterCount => _importer._parameterCount;
            public IList<LocalVariableDescriptor> LocalVariableTable => _importer._localVariableTable;

            public BasicBlock[] BasicBlocks => _importer._basicBlocks;

            public int? ReturnBufferArgIndex => _importer._returnBufferArgIndex;

            public void ImportAppendTree(StackEntry entry, bool spill = false) => _importer.ImportAppendTree(entry, spill);
            public void ImportFallThrough(BasicBlock next) => _importer.ImportFallThrough(next);
            public StackEntry PopExpression() => _importer._stack.Pop();
            public void PushExpression(StackEntry entry) => _importer._stack.Push(entry);
                
            public int GrabTemp(VarType type, int? exactSize) => _importer.GrabTemp(type, exactSize);
        }

        public ILImporter(IConfiguration configuration, ILogger<ILImporter> logger, INameMangler nameMangler, IEnumerable<IOpcodeImporter> importers, CorLibModuleProvider corlibModuleProvider)
        {
            _configuration = configuration;
            _basicBlocks = Array.Empty<BasicBlock>();
            _logger = logger;
            _nameMangler = nameMangler;
            _importers = importers;
            _corLibModuleProvider = corlibModuleProvider;

            _importerProxy = new ILImporterProxy(this);
        }

        private void ImportBasicBlocks(IDictionary<int, int> offsetToIndexMap)
        {
            _pendingBasicBlocks = _basicBlocks[0];
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
        }

        private void EndImportingBasicBlock(BasicBlock basicBlock)
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

        private void ImportAppendTree(StackEntry entry, bool spill = false)
        {
            if (spill)
            {
                ImportSpillAppendTree(entry);
            }
            else
            {
                _currentBasicBlock?.Statements.Add(entry);
            }
        }

        private StackEntry? ImportExtractLastStmt()
        {
            StackEntry? lastStmt = null;
            if (_currentBasicBlock?.Statements.Count > 0)
            {
                var lastStmtIndex = _currentBasicBlock.Statements.Count - 1;
                lastStmt = _currentBasicBlock.Statements[lastStmtIndex];
                _currentBasicBlock.Statements.RemoveAt(lastStmtIndex);
            }
            return lastStmt;
        }

        private void ImportBasicBlock(IDictionary<int, int> offsetToIndexMap, BasicBlock block)
        {
            _currentBasicBlock = block;
            var currentOffset = block.StartOffset;
            var currentIndex = offsetToIndexMap[currentOffset];

            while (true)
            {
                var currentInstruction = _method.Body.Instructions[currentIndex];
                currentOffset += currentInstruction.GetSize();
                currentIndex++;

                var opcode = currentInstruction.OpCode.Code;
                var fallthroughBlock = currentOffset < _basicBlocks.Length ? _basicBlocks[currentOffset] : null;
                var importContext = new ImportContext(block, fallthroughBlock, _method, _nameMangler, _corLibModuleProvider);

                var imported = false;
                foreach (var importer in _importers)
                {
                    if (importer.Import(currentInstruction, importContext, _importerProxy))
                    {
                        imported = true;
                        break;
                    }
                }

                if (imported)
                {
                    if (importContext.StopImporting)
                    {
                        return;
                    }
                }
                else if (_configuration.IgnoreUnknownCil)
                {
                    _logger.LogWarning("Unsupported IL opcode {opcode}", opcode);
                }
                else
                {
                    throw new UnknownCilException($"Unsupported IL opcode {opcode}");
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

        private int GrabTemp(VarType type, int? exactSize)
        {
            var temp = new LocalVariableDescriptor()
            {
                IsParameter = false,
                IsTemp = true,
                ExactSize = exactSize ?? 0,
                Type = type
            };

            _localVariableTable.Add(temp);

            return _localVariableTable.Count - 1;
        }

        private StackEntry ImportSpillStackEntry(StackEntry entry, int? tempNumber = null)
        {
            if (tempNumber == null)
            {
                tempNumber = GrabTemp(entry.Type, entry.ExactSize);
            }

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

        public IList<BasicBlock> Import(int parameterCount, int? returnBufferArgIndex, MethodDesc method, IList<LocalVariableDescriptor> localVariableTable)
        {
            _parameterCount = parameterCount;
            _returnBufferArgIndex = returnBufferArgIndex;

            _method = method;
            _localVariableTable = localVariableTable;

            var basicBlockAnalyser = new BasicBlockAnalyser(_method);
            var offsetToIndexMap = new Dictionary<int, int>();
            _basicBlocks = basicBlockAnalyser.FindBasicBlocks(offsetToIndexMap);

            ImportBasicBlocks(offsetToIndexMap);

            var importedBasicBlocks = new List<BasicBlock>();
            for (int i = 0; i < _basicBlocks.Length; i++)
            {
                if (_basicBlocks[i] != null)
                {
                    importedBasicBlocks.Add(_basicBlocks[i]);
                }
            }

            return importedBasicBlocks;
        }

        private void ImportFallThrough(BasicBlock next)
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
                var lastStmt = ImportExtractLastStmt();

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
                if (lastStmt != null)
                {
                    ImportAppendTree(lastStmt);
                }

                // Set the entry stack for the next basic block to the one we've built
                // e.g. with the new temps on it
                next.EntryStack = entryStack;
            }

            MarkBasicBlock(next);
        }
    }
}