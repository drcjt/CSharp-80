using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.Importer;
using ILCompiler.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ILCompiler.Compiler
{
    public class ILImporter : IILImporter
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ILImporter> _logger;
        private readonly INameMangler _nameMangler;

        private MethodDef _method = null!;
        private IList<LocalVariableDescriptor> _localVariableTable = null!;

        private BasicBlock[] _basicBlocks;
        private BasicBlock? _currentBasicBlock;
        private BasicBlock? _pendingBasicBlocks;

        private int _parameterCount;
        private int? _returnBufferArgIndex;

        private readonly EvaluationStack<StackEntry> _stack = new EvaluationStack<StackEntry>(0);

        private readonly IOpcodeImporterFactory _importerFactory;
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

            public void ImportAppendTree(StackEntry entry) => _importer.ImportAppendTree(entry);
            public void ImportFallThrough(BasicBlock next) => _importer.ImportFallThrough(next);
            public StackEntry PopExpression() => _importer._stack.Pop();
            public void PushExpression(StackEntry entry) => _importer._stack.Push(entry);

            public int GrabTemp(StackValueKind kind, int? exactSize) => _importer.GrabTemp(kind, exactSize);
        }

        public ILImporter(IConfiguration configuration, ILogger<ILImporter> logger, INameMangler nameMangler, IOpcodeImporterFactory importerFactory)
        {
            _configuration = configuration;
            _basicBlocks = Array.Empty<BasicBlock>();
            _logger = logger;
            _nameMangler = nameMangler;
            _importerFactory = importerFactory;

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

        private void ImportAppendTree(StackEntry entry)
        {
            _currentBasicBlock?.Statements.Add(entry);
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

                var importer = _importerFactory.GetImporter(opcode);

                if (importer != null)
                {
                    var currentBlock = currentOffset < _basicBlocks.Length ? _basicBlocks[currentOffset] : null;
                    var importContext = new ImportContext(currentBlock, _method, _nameMangler);
                    importer.Import(currentInstruction, importContext, _importerProxy);
                    if (importContext.StopImporting)
                    {
                        return;
                    }
                }   
                else  if (_configuration.IgnoreUnknownCil)
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

        private int GrabTemp(StackValueKind kind, int? exactSize)
        {
            var temp = new LocalVariableDescriptor()
            {
                IsParameter = false,
                IsTemp = true,
                Kind = kind,
                ExactSize = exactSize ?? 0,
            };

            _localVariableTable.Add(temp);

            return _localVariableTable.Count - 1;
        }

        private StackEntry ImportSpillStackEntry(StackEntry entry, int? tempNumber = null)
        {
            if (tempNumber == null)
            {
                tempNumber = GrabTemp(entry.Kind, entry.ExactSize);
                var temp = _localVariableTable[tempNumber.Value];
            }

            var node = new StoreLocalVariableEntry(tempNumber.Value, false, entry);
            ImportAppendTree(node);

            return new LocalVariableEntry(tempNumber.Value, entry.Kind, entry.ExactSize);
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

        public IList<BasicBlock> Import(int parameterCount, int? returnBufferArgIndex, MethodDef method, IList<LocalVariableDescriptor> localVariableTable)
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
            // Evaluation stack in each basic block holds the imported high level tree representation of the IL

            EvaluationStack<StackEntry>? entryStack = next.EntryStack;

            if (entryStack != null)
            {
                // Check the entry stack and the current stack are equivalent,
                // i.e. have same length and elements are identical

                if (entryStack.Length != _stack.Length)
                {
                    throw new InvalidProgramException();
                }

                for (int i = 0; i < entryStack.Length; i++)
                {
                    if (entryStack[i].Kind != _stack[i].Kind)
                    {
                        throw new InvalidProgramException();
                    }

                    if (entryStack[i].Kind == StackValueKind.ValueType)
                    {
                        if (entryStack[i].Kind != _stack[i].Kind)
                            throw new InvalidProgramException();
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