using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.OpcodeImporters;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
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

        private int _currentILOffset = 0;


        private readonly CodeFolder _codeFolder;
        public CodeFolder CodeFolder => _codeFolder;

        public Importer(IConfiguration configuration, ILogger<Importer> logger, INameMangler nameMangler, IEnumerable<IOpcodeImporter> importers, PreinitializationManager preinitializationManager, NodeFactory nodeFactory, CodeFolder codeFolder)
        {
            Configuration = configuration;
            NameMangler = nameMangler;
            PreinitializationManager = preinitializationManager;
            NodeFactory = nodeFactory;
            Logger = logger;
            _opcodeImporters = importers;
            _codeFolder = codeFolder;
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
                var statement = new Statement(entry)
                {
                    StartOffset = _currentILOffset,
                };
                _currentBasicBlock?.Statements.Add(statement);
            }
        }

        private StackEntry? ImportExtractLastStatement()
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
            _currentILOffset = block.StartOffset;
            var currentIndex = offsetToIndexMap[_currentILOffset];

            // Add exception object for catch
            if (block.HandlerStart)
            {
                // Use a node type that won't generate any code
                var catchArgument = new CatchArgumentEntry();

                // Spill to a temp to ensure correct evaluation order
                var temp = ImportSpillStackEntry(catchArgument);
                _stack.Push(temp);
            }

            StopImporting = false;

            while (true)
            {
                var currentInstruction = _methodIL!.Instructions[currentIndex];
                _currentILOffset += currentInstruction.GetSize();
                currentIndex++;

                FallThroughBlock = _currentILOffset < BasicBlocks.Length ? BasicBlocks[_currentILOffset] : null;

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

                if (_currentILOffset == BasicBlocks.Length)
                {
                    return;
                }

                var nextBasicBlock = BasicBlocks[_currentILOffset];
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
            tempNumber = tempNumber ?? GrabTemp(entry.Type, entry.ExactSize);

            var node = new StoreLocalVariableEntry(tempNumber.Value, false, entry);
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

            var basicBlockAnalyser = new BasicBlockAnalyser(Method, NameMangler, _methodIL, this);
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
                // If the stack is not empty on exit from the basic block, additional
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
                var lastStatement = ImportExtractLastStatement();

                // Spill stuff on stack in new temps and setup new stack to contain the temps
                for (int i = 0; i < _stack.Length; i++)
                {
                    int? tempNumber = null;
                    if (!setupEntryStack)
                    {
                        tempNumber = (entryStack[i].As<LocalVariableEntry>()).LocalNumber;
                    }

                    if (!_currentBasicBlock.SpilledStackEntries.TryGetValue(_stack[i], out LocalVariableEntry? temp))
                    {
                        temp = ImportSpillStackEntry(_stack[i], tempNumber);
                        _currentBasicBlock.SpilledStackEntries[_stack[i]] = temp;
                    }

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
            var argumentCanBeModified = inlineArgumentInfo.HasLdargaOp || inlineArgumentInfo.HasStargOp;

            StackEntry result;
            if (inlineArgumentInfo.IsInvariant && !argumentCanBeModified)
            {
                // Directly substitute constants or addresses of locals
                result = inlineArgumentInfo.Argument.Duplicate();
            }
            else if (inlineArgumentInfo.IsLocalVariable && !argumentCanBeModified)
            {
                // Directly substitute unaliased caller locals for arguments that cannot be modified

                // Use the caller supplied node if this is the first use
                result = inlineArgumentInfo.Argument;

                // Use an equivalent copy if this is the second or later use
                if (inlineArgumentInfo.IsUsed)
                {
                    var argLocal = inlineArgumentInfo.Argument.As<LocalVariableEntry>();
                    result = new LocalVariableEntry(argLocal.LocalNumber, argLocal.Type, argLocal.ExactSize);
                }
            }
            else
            {

                if (inlineArgumentInfo.HasTemp)
                {
                    var callArgument = inlineArgumentInfo.Argument;
                    var tempNumber = inlineArgumentInfo.TempNumber;
                    result = new LocalVariableEntry(tempNumber, callArgument.Type, callArgument.ExactSize);
                }
                else
                {
                    var callArgument = inlineArgumentInfo.Argument;

                    var tempNumber = GrabTemp(callArgument.Type, callArgument.ExactSize);

                    inlineArgumentInfo.HasTemp = true;
                    inlineArgumentInfo.TempNumber = tempNumber;

                    result = new LocalVariableEntry(tempNumber, callArgument.Type, callArgument.ExactSize);
                }
            }

            inlineArgumentInfo.IsUsed = true;

            return result;
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
                var tempNumber = GrabTemp(localType!);
                inlineLocalInfo.HasTemp = true;
                inlineLocalInfo.TempNumber = tempNumber;

                return tempNumber;
            }
        }

        public int GrabTemp(TypeDesc type) => GrabTemp(type.VarType, type.GetElementSize().AsInt);

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

        public int MapIlArgNum(int ilArgNum)
        {
            if (ReturnBufferArgIndex.HasValue && ilArgNum >= ReturnBufferArgIndex)
            {
                ilArgNum++;
            }

            return ilArgNum;
        }

        public StackEntry StoreStruct(StackEntry node)
        {
            if (node is IStoreEntry store)
            {
                if (store.Op1 is CallEntry call)
                {
                    Debug.Assert(call.Method != null);
                    Debug.Assert(call.Method.HasReturnType);
                    if (call.Method.Signature.ReturnType.VarType == VarType.Struct)
                    {
                        // If the call returns a struct, we need to store it in a local variable
                        var destinationAddress = GetNodeAddress(node);

                        // Insert the return buffer into the argument list of the call as first byref parameter after the this pointer
                        var hasThis = call.Method!.HasThis;
                        call.Arguments.Insert(hasThis ? 1 : 0, destinationAddress);
                        call.HasReturnBuffer = true;
                    }

                    return call;
                }
                else if (store.Op1 is ReturnExpressionEntry returnExpression)
                {
                    var inlineCall = returnExpression.InlineCandidate;

                    var destinationAddress = GetNodeAddress(node);

                    // Insert the return buffer into the argument list of the call as first byref parameter after the this pointer
                    var hasThis = inlineCall.Method!.HasThis;
                    inlineCall.Arguments.Insert(hasThis ? 1 : 0, destinationAddress);
                    inlineCall.HasReturnBuffer = true;

                    inlineCall.SetReturnType(VarType.Void);

                    return returnExpression;
                }
                else if (store.Op1 is CommaEntry comma)
                {
                    // TODO: Handle comma entry
                    throw new NotImplementedException("StoreStruct does not support CommaEntry yet");
                }
            }

            return node;
        }

        public StackEntry GetNodeAddress(StackEntry value)
        {
            if (value is IndirectEntry indirectEntry)
            {
                return indirectEntry.Op1;
            }
            if (value is StoreIndEntry storeIndirect)
            {
                return storeIndirect.Addr;
            }
            else if (value is LocalVariableEntry localVariable)
            {
                return new LocalVariableAddressEntry(localVariable.LocalNumber);
            }
            else if (value is StoreLocalVariableEntry storeLocalVariable)
            {
                return new LocalVariableAddressEntry(storeLocalVariable.LocalNumber);
            }
            else
            {
                var localNumber = GrabTemp(value.Type, value.ExactSize);
                StackEntry storeToTemp = NewTempStore(localNumber, value);
                ImportAppendTree(storeToTemp, true);

                return new LocalVariableAddressEntry(localNumber);
            }
        }

        public StackEntry NewTempStore(int tempNumber, StackEntry value)
        {
            StackEntry store = new StoreLocalVariableEntry(tempNumber, false, value);
            if (value.Type == VarType.Struct)
            {
                store = StoreStruct(store);
            }

            return store;
        }
    }
}
