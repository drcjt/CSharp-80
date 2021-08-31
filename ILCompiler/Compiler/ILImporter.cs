﻿using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.IL;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.Importer;
using ILCompiler.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ILCompiler.Compiler
{
    public class ILImporter : IILImporter
    {
        private readonly MethodCompiler _methodCompiler;
        private readonly MethodDef _method;
        private readonly IList<LocalVariableDescriptor> _localVariableTable;
        private readonly IConfiguration _configuration;

        private BasicBlock[] _basicBlocks;
        private BasicBlock _currentBasicBlock;
        private BasicBlock _pendingBasicBlocks;

        public INameMangler NameMangler => _methodCompiler.NameMangler;

        private readonly EvaluationStack<StackEntry> _stack = new EvaluationStack<StackEntry>(0);

        private readonly IList<IOpcodeImporter> _importers = new List<IOpcodeImporter>();

        public int ParameterCount { get => _methodCompiler.ParameterCount; }
        public IList<LocalVariableDescriptor> LocalVariableTable { get => _localVariableTable; }
        public BasicBlock[] BasicBlocks { get => _basicBlocks; }

        public ILImporter(MethodCompiler methodCompiler, MethodDef method, IList<LocalVariableDescriptor> localVariableTable, IConfiguration configuration)
        {
            _methodCompiler = methodCompiler;
            _method = method;
            _localVariableTable = localVariableTable;
            _configuration = configuration;

            _importers.Add(new NopImporter());
            _importers.Add(new LoadIntImporter(this));
            _importers.Add(new StoreVarImporter(this));
            _importers.Add(new LoadVarImporter(this));
            _importers.Add(new AddressOfVarImporter(this));
            _importers.Add(new StoreIndirectImporter(this));
            _importers.Add(new LoadIndirectImporter(this));
            _importers.Add(new StoreFieldImporter(this));
            _importers.Add(new LoadFieldImporter(this));
            _importers.Add(new BinaryOperationImporter(this));
            _importers.Add(new CompareImporter(this));
            _importers.Add(new BranchImporter(this));
            _importers.Add(new LdArgImporter(this));
            _importers.Add(new StArgImporter(this));
            _importers.Add(new LoadStringImporter(this));
            _importers.Add(new InitObjImporter(this));
            _importers.Add(new ConversionImporter(this));
            _importers.Add(new NegImporter(this));
            _importers.Add(new RetImporter(this));
            _importers.Add(new CallImporter(this));
        }

        private void ImportBasicBlocks(IDictionary<int, int> offsetToIndexMap)
        {
            _pendingBasicBlocks = _basicBlocks[0];
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

            EvaluationStack<StackEntry> entryStack = basicBlock.EntryStack;
            if (entryStack != null)
            {
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

        public void ImportAppendTree(StackEntry entry)
        {
            _currentBasicBlock.Statements.Add(entry);
        }

        private StackEntry ImportExtractLastStmt()
        {
            var lastStmtIndex = _currentBasicBlock.Statements.Count - 1;
            var lastStmt = _currentBasicBlock.Statements[lastStmtIndex];
            _currentBasicBlock.Statements.RemoveAt(lastStmtIndex);
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

                var importer = _importers.FirstOrDefault(importer => importer.CanImport(opcode));

                if (importer != null)
                {
                    var importContext = new ImportContext()
                    {
                        CurrentBlock = currentOffset < _basicBlocks.Length ? _basicBlocks[currentOffset] : null,
                        Method = _method,
                        NameMangler = _methodCompiler.NameMangler,
                    };
                    importer.Import(currentInstruction, importContext);
                    if (importContext.StopImporting)
                    {
                        return;
                    }
                }   
                else  if (_configuration.IgnoreUnknownCil)
                {
                    _methodCompiler.Logger.LogWarning($"Unsupported IL opcode {opcode}");
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

        private int GrabTemp()
        {
            var temp = new LocalVariableDescriptor()
            {
                IsParameter = false,
                IsTemp = true,
            };

            _localVariableTable.Add(temp);

            return _localVariableTable.Count - 1;
        }

        private StackEntry ImportSpillStackEntry(StackEntry entry, int? tempNumber = null)
        {
            if (tempNumber == null)
            {
                tempNumber = GrabTemp();
                var temp = _localVariableTable[tempNumber.Value];
                temp.Kind = entry.Kind;
                temp.ExactSize = TypeList.GetExactSize(entry.Kind);
                temp.StackOffset = tempNumber == 0 ? 0 : _localVariableTable[tempNumber.Value - 1].StackOffset + temp.ExactSize;
            }

            var node = new StoreLocalVariableEntry(tempNumber.Value, false, entry);
            ImportAppendTree(node);

            return new LocalVariableEntry(tempNumber.Value, entry.Kind);
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

        public void PushExpression(StackEntry expression)
        {
            _stack.Push(expression);
        }

        public StackEntry PopExpression()
        {
            return _stack.Pop();
        }

        public IList<BasicBlock> Import()
        {
            var basicBlockAnalyser = new BasicBlockAnalyser(_method);
            var offsetToIndexMap = basicBlockAnalyser.FindBasicBlocks();
            _basicBlocks = basicBlockAnalyser.BasicBlocks;

            ImportBasicBlocks(offsetToIndexMap);

            var basicBlocks = new List<BasicBlock>();
            for (int i = 0; i < _basicBlocks.Length; i++)
            {
                if (_basicBlocks[i] != null)
                {
                    basicBlocks.Add(_basicBlocks[i]);
                }
            }

            return basicBlocks;
        }

        public void ImportFallThrough(BasicBlock next)
        {
            // Evaluation stack in each basic block holds the imported high level tree representation of the IL

            EvaluationStack<StackEntry> entryStack = next.EntryStack;

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
                // Stack is not empty at end of basic block
                // So we must spill stack into temps
                // Anything on the stack effectively gets turned into assignments to
                // temporary local variables
                // And successor basic blocks will have these temporary local variables
                // on the stack on entry

                var setupEntryStack = entryStack == null;
                if (setupEntryStack)
                {
                    entryStack = new EvaluationStack<StackEntry>(_stack.Length);
                }

                var lastStmt = ImportExtractLastStmt();

                for (int i = 0; i < _stack.Length; i++)
                {
                    int? tempNumber = null;
                    if (!setupEntryStack)
                    {
                        tempNumber = (entryStack[i] as LocalVariableEntry).LocalNumber;
                    }
                    var temp = ImportSpillStackEntry(_stack[i], tempNumber);
                    if (setupEntryStack)
                    {
                        entryStack.Push(temp);
                    }
                }
                ImportAppendTree(lastStmt);

                next.EntryStack = entryStack;
            }

            MarkBasicBlock(next);
        }
    }
}