﻿using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.Dnlib;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.Importer;
using ILCompiler.Interfaces;
using Microsoft.Extensions.Logging;
using PreinitializationManager = ILCompiler.Compiler.PreInit.PreinitializationManager;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.Compiler
{
    public class ILImporter : IILImporter
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ILImporter> _logger;
        private readonly INameMangler _nameMangler;
        private readonly CorLibModuleProvider _corLibModuleProvider;
        private readonly PreinitializationManager _preinitializationManager;
        private readonly NodeFactory _nodeFactory;
        private readonly DnlibModule _module;

        private MethodDesc _method = null!;
        private MethodIL _methodIL = null!;
        private LocalVariableTable? _locals;

        private BasicBlock[] _basicBlocks;
        private BasicBlock? _currentBasicBlock;
        private BasicBlock? _pendingBasicBlocks;

        private int _parameterCount;
        private int? _returnBufferArgIndex;

        private readonly EvaluationStack<StackEntry> _stack = new EvaluationStack<StackEntry>(0);

        private readonly IEnumerable<IOpcodeImporter> _importers;
        private readonly ILImporterProxy _importerProxy;

        private bool _inlining;

        private sealed class ILImporterProxy : IILImporterProxy
        {
            private readonly ILImporter _importer;
            public ILImporterProxy(ILImporter importer)
            {
                _importer = importer;
            }

            public int ParameterCount => _importer._parameterCount;
            public LocalVariableTable LocalVariableTable => _importer._locals!;

            public BasicBlock[] BasicBlocks => _importer._basicBlocks;

            public int? ReturnBufferArgIndex => _importer._returnBufferArgIndex;

            public void ImportAppendTree(StackEntry entry, bool spill = false) => _importer.ImportAppendTree(entry, spill);
            public void ImportFallThrough(BasicBlock next) => _importer.ImportFallThrough(next);
            public StackEntry PopExpression() => _importer._stack.Pop();
            public void PushExpression(StackEntry entry) => _importer._stack.Push(entry);
                
            public int GrabTemp(VarType type, int? exactSize) => _importer._locals!.GrabTemp(type, exactSize);
        }

        public ILImporter(IConfiguration configuration, ILogger<ILImporter> logger, INameMangler nameMangler, IEnumerable<IOpcodeImporter> importers, CorLibModuleProvider corlibModuleProvider, PreinitializationManager preinitializationManager, NodeFactory nodeFactory, DnlibModule module)
        {
            _configuration = configuration;
            _basicBlocks = Array.Empty<BasicBlock>();
            _logger = logger;
            _nameMangler = nameMangler;
            _importers = importers;
            _corLibModuleProvider = corlibModuleProvider;
            _preinitializationManager = preinitializationManager;
            _nodeFactory = nodeFactory;
            _module = module;

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

        private void ImportAppendTree(StackEntry entry, bool spill = false)
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

        private StackEntry? ImportExtractLastStmt()
        {
            if (_currentBasicBlock?.Statements.Count > 0)
            {
                var lastStatementIndex = _currentBasicBlock.Statements.Count - 1;
                var lastStmt = _currentBasicBlock.Statements[lastStatementIndex];
                var lastStmtRoot = lastStmt.RootNode;

                if (lastStmtRoot is JumpEntry || lastStmtRoot is JumpTrueEntry)
                {
                    // Remove the branch statement from the current basic block
                    _currentBasicBlock.Statements.RemoveAt(lastStatementIndex);
                    return lastStmtRoot;
                }
            }

            return null;
        }

        private void ImportBasicBlock(IDictionary<int, int> offsetToIndexMap, BasicBlock block)
        {
            _currentBasicBlock = block;
            var currentOffset = block.StartOffset;
            var currentIndex = offsetToIndexMap[currentOffset];

            var importContext = new ImportContext
            {
                CurrentBlock = block,
                Method = _method,
                NameMangler = _nameMangler,
                Configuration = _configuration,
                CorLibModuleProvider = _corLibModuleProvider,
                PreinitializationManager = _preinitializationManager,
                NodeFactory = _nodeFactory,
                Module = _module,
                Inlining = _inlining,
            };

            while (true)
            {
                var currentInstruction = _methodIL!.Instructions[currentIndex];
                currentOffset += currentInstruction.GetSize();
                currentIndex++;

                importContext.FallThroughBlock = currentOffset < _basicBlocks.Length ? _basicBlocks[currentOffset] : null;

                bool imported = ImportInstruction(currentInstruction, importContext);

                if (imported)
                {
                    if (importContext.StopImporting)
                    {
                        return;
                    }
                }
                else
                {
                    HandleUnknownInstruction(currentInstruction);
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

        private void HandleUnknownInstruction(Instruction currentInstruction)
        {
            if (_configuration.IgnoreUnknownCil)
            {
                _logger.LogWarning("Unsupported IL opcode {Opcode} in {MethodFullName}", currentInstruction.Opcode, _method.FullName);
            }
            else
            {
                throw new UnknownCilException($"Unsupported IL opcode {currentInstruction.Opcode} in {_method.FullName}");
            }
        }

        private bool ImportInstruction(Instruction currentInstruction, ImportContext importContext)
        {
            var imported = false;
            foreach (var importer in _importers)
            {
                if (importer.Import(currentInstruction, importContext, _importerProxy))
                {
                    imported = true;
                    break;
                }
            }

            return imported;
        }

        private StackEntry ImportSpillStackEntry(StackEntry entry, int? tempNumber = null)
        {
            if (tempNumber == null)
            {
                tempNumber = _locals!.GrabTemp(entry.Type, entry.ExactSize);
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

        public IList<BasicBlock> Import(int parameterCount, int? returnBufferArgIndex, MethodDesc method, LocalVariableTable locals, IList<EHClause> ehClauses, bool inlining = false)
        {
            _inlining = inlining;

            _parameterCount = parameterCount;
            _returnBufferArgIndex = returnBufferArgIndex;

            _method = method;
            _locals = locals;

            var methodIL = method.MethodIL!;

            var uninstantiatedMethodIL = methodIL.GetMethodILDefinition();
            if (methodIL != uninstantiatedMethodIL)
            {
                var sharedMethod = _method.GetSharedRuntimeFormMethodTarget();
                _methodIL = new InstantiatedMethodIL(sharedMethod, uninstantiatedMethodIL);
            }
            else
            {
                _methodIL = methodIL;
            }

            var basicBlockAnalyser = new BasicBlockAnalyser(_method, _nameMangler, _methodIL);
            var offsetToIndexMap = new Dictionary<int, int>();
            _basicBlocks = basicBlockAnalyser.FindBasicBlocks(offsetToIndexMap, ehClauses);

            // Trigger static constructor if required
            if (method.IsDefaultConstructor)
            {
                var staticConstructorMethod = method.OwningType.GetStaticConstructor();
                if (staticConstructorMethod != null)
                {
                    var targetMethod = _nameMangler.GetMangledMethodName(staticConstructorMethod);
                    var staticInitCall = new CallEntry(targetMethod, new List<StackEntry>(), VarType.Void, 0);

                    _basicBlocks[0].Statements.Add(new Statement(staticInitCall));
                }
            }

            ImportBasicBlocks(offsetToIndexMap);

            var importedBasicBlocks = new List<BasicBlock>();
            for (int i = 0; i < _basicBlocks.Length; i++)
            {
                if (_basicBlocks[i] != null)
                {                    
                    importedBasicBlocks.Add(_basicBlocks[i]);
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
                var lastStatement = ImportExtractLastStmt();

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
    }
}