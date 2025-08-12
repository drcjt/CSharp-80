using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Compiler.OpcodeImporters;
using ILCompiler.Compiler.PreInit;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using StackEntry = ILCompiler.Compiler.EvaluationStack.StackEntry;

namespace ILCompiler.Compiler
{
    public record InlineMethodInfo
    {
        public required CallEntry Call { get; set; }
        public required Statement Statement { get; set; }
        public required int StatementIndex { get; set; }
        public required BasicBlock Block { get; set; }
        public required LocalVariableTable Locals { get; set; }
    }

    public record InlineArgumentInfo
    {
        public required StackEntry Argument {  get; set; }
        public int TempNumber { get; set; }
        public bool HasTemp { get; set; }
        public bool IsInvariant { get; set; }
        public bool IsLocalVariable { get; set; }
        public bool IsUsed { get; set; }
        public bool HasLdargaOp { get; set; }
        public bool HasStargOp { get; set; }
    }

    public record LocalVariableInfo
    {
        public int TempNumber { get; set; }
        public bool HasTemp { get; set; }
        public TypeDesc? Type { get; set; }
    }

    public record InlineInfo
    {
        public required CallEntry InlineCall { get; set; }
        public InlineArgumentInfo[] InlineArgumentInfos { get; set; } = [];
        public LocalVariableInfo[] LocalVariableInfos { get; set; } = [];
        public required LocalVariableTable InlineLocalVariableTable { get; set; }

        public int? InlineeReturnSpillTempNumber { get; set; } = null;

        public InlineCandidateInfo? InlineCandidateInfo { get; set; } = null;
        public MethodDesc? StaticConstructorMethod { get; set; } = null;
    }

    public record InlineCandidateInfo
    {
        public ReturnExpressionEntry? ReturnExpressionEntry { get; set; } = null;
    }

    public class SubstitutePlaceholdersWalker : StackEntryVisitor
    {
        public SubstitutePlaceholdersWalker() : base() 
        {
        }

        public Statement WalkStatement(Statement statement)
        {
            WalkTree(new Edge<StackEntry>(() => statement.RootNode, x => { statement.RootNode = x; }), null);

            return statement;
        }

        public override void PreOrderVisit(Edge<StackEntry> use, StackEntry? user)
        {
            var node = use.Get();
            if (node is ReturnExpressionEntry)
            {
                UpdateInlineReturnExpressionPlaceHolder(use, user);
            }
        }

        public override void PostOrderVisit(Edge<StackEntry> use, StackEntry? user)
        {
            use.Set(CodeFolder.FoldExpression(use.Get()!));
        }

        private static void UpdateInlineReturnExpressionPlaceHolder(Edge<StackEntry> use, StackEntry? user)
        {
            while (use.Get() is ReturnExpressionEntry)
            {
                var tree = use.Get();
                var inlineCandidate = tree;

                do
                {
                    var returnExpression = inlineCandidate as ReturnExpressionEntry;
                    inlineCandidate = returnExpression!.SubstitutionExpression;
                } while (inlineCandidate is ReturnExpressionEntry);

                inlineCandidate = CodeFolder.FoldExpression(inlineCandidate!);

                use.Set(inlineCandidate!);
            }
        }
    }

    public class Inliner(ILogger<MethodCompiler> logger, IConfiguration configuration, IPhaseFactory phaseFactory, INameMangler nameMangler, PreinitializationManager preinitializationManager) : IInliner
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<MethodCompiler> _logger = logger;
        private readonly IPhaseFactory _phaseFactory = phaseFactory;
        private readonly INameMangler _nameMangler = nameMangler;
        private readonly PreinitializationManager _preinitializationManager = preinitializationManager;
        private string? _inputFilePath;

        private IList<BasicBlock>? _blocks;

        public void Inline(IList<BasicBlock> blocks, LocalVariableTable locals, string inputFilePath)
        {
            if (!_configuration.Optimize)
                return;

            _blocks = blocks;
            _inputFilePath = inputFilePath;

            var walker = new SubstitutePlaceholdersWalker();

            var blockIndex = 0;

            do
            {
                var block = blocks[blockIndex];
                if (block.Statements.Count > 0)
                {
                    int statementIndex = 0;
                    do
                    {
                        var statement = block.Statements[statementIndex];

                        // Walk Statement
                        statement = walker.WalkStatement(statement);

                        var expr = statement.RootNode;

                        var callInlined = false;
                        if (expr is CallEntry call && call.IsInlineCandidate)
                        {
                            var inlineMethodInfo = new InlineMethodInfo()
                            {
                                Call = call,
                                Statement = statement,
                                StatementIndex = statementIndex,
                                Block = block,
                                Locals = locals,
                            };
                            MorphCallInline(inlineMethodInfo);
                        }

                        // Skip over the statement that has either not been inlined
                        // or has been inlined and replaced with a NothingEntry
                        statementIndex++;
                    } while (statementIndex < block.Statements.Count);
                }

                blockIndex++;
            } while (blockIndex < blocks.Count);
        }

        private void MorphCallInline(InlineMethodInfo methodInfo)
        {
            var method = methodInfo.Call.Method;
            if (method is null)
            {
                throw new InvalidOperationException("Method is null");
            }

            InlineInfo inlineInfo = InitVars(methodInfo);
            inlineInfo.InlineCandidateInfo = methodInfo.Call.InlineCandidateInfo;

            var staticConstructorMethod = method.GetStaticConstructor();
            if (staticConstructorMethod is not null && !_preinitializationManager.IsPreinitialized(method.OwningType))
            {
                inlineInfo.StaticConstructorMethod = method.GetStaticConstructor();
            }

            var startVars = methodInfo.Locals.Count;

            var compiler = new MethodCompiler(_logger, _configuration, _phaseFactory);
            var basicBlocks = compiler.CompileInlineeMethod(method, _inputFilePath!, inlineInfo);

            if (basicBlocks != null)
            {
                var inlineSucceeded = InsertInlineeBlocks(basicBlocks, methodInfo, inlineInfo);
                if (inlineSucceeded)
                {
                    _logger.LogInformation("Inlined call to method: {MethodName}", method.FullName);
                    return;
                }

                if (method.HasReturnType)
                {
                    inlineInfo.InlineCandidateInfo!.ReturnExpressionEntry!.SubstitutionExpression = methodInfo.Call;

                    // Need to blank out the original call node
                    methodInfo.Statement!.RootNode = new NothingEntry();
                }

                // Need to undo some changes made during the inlining attempt
                // Temps may have been allocated need to do this
                methodInfo.Locals.ResetCount(startVars);

                inlineInfo.InlineCall.IsInlineCandidate = false;
            }

            Debug.Assert(methodInfo.Locals.Count == startVars);
        }

        private static InlineInfo InitVars(InlineMethodInfo methodInfo)
        {
            int returnBufferArgIndex = -1;
            if (methodInfo.Call.HasReturnBuffer)
            {
                var hasThis = methodInfo.Call.Method!.HasThis;
                returnBufferArgIndex = hasThis ? 1 : 0;
            }

            var argumentCount = methodInfo.Call.Arguments.Count - (methodInfo.Call.HasReturnBuffer ? 1 : 0);

            var inlineInfo = new InlineInfo
            {
                InlineCall = methodInfo.Call,
                InlineLocalVariableTable = methodInfo.Locals,
                InlineArgumentInfos = new InlineArgumentInfo[argumentCount]
            };

            int inlineArgumentIndex = 0;
            for (int i = 0; i < methodInfo.Call.Arguments.Count; i++)
            {
                if (returnBufferArgIndex >= 0 && i == returnBufferArgIndex)
                {
                    // Skip the return buffer argument
                    continue;
                }

                var argument = methodInfo.Call.Arguments[i];

                if (argument is PutArgTypeEntry putArgType)
                {
                    argument = putArgType.Op1;
                }
                var inlineArgumentInfo = new InlineArgumentInfo()
                {
                    Argument = argument,
                };

                RecordArgumentInfo(argument, inlineArgumentInfo);

                inlineInfo.InlineArgumentInfos[inlineArgumentIndex++] = inlineArgumentInfo;
            }

            var method = methodInfo.Call.Method!;
            inlineInfo.LocalVariableInfos = new LocalVariableInfo[method.Locals.Count];

            for (int i = 0; i < method.Locals.Count; i++)
            {
                var local = method.Locals[i];
                var localVariableInfo = new LocalVariableInfo()
                {
                    Type = local.Type,
                };
                inlineInfo.LocalVariableInfos[i] = localVariableInfo;
            }

            return inlineInfo;
        }

        private static void RecordArgumentInfo(StackEntry? argument, InlineArgumentInfo argumentInfo)
        {
            argumentInfo.IsInvariant = argument!.IsInvariant();
            argumentInfo.IsLocalVariable = argument is LocalVariableEntry;
        }

        private bool InsertInlineeBlocks(IList<BasicBlock> blocks, InlineMethodInfo methodInfo, InlineInfo inlineInfo)
        {
            int afterStatementIndex = methodInfo.StatementIndex;
            var statements = methodInfo.Block.Statements;

            // Don't inline calls in blocks that have exception handlers
            // when we have multiple blocks in the inlinee
            if (blocks.Count > 1 && methodInfo.Block.Handlers.Count > 0)
                return false;

            // Prepend statements
            PrependStatements(methodInfo, inlineInfo, ref afterStatementIndex);

            bool insertInlineeBlocks = true;
            if (blocks.Count == 1)
            {
                var inlineeBlock = blocks[0];
                foreach (var inlineeStatement in inlineeBlock.Statements)
                {
                    statements.Insert(afterStatementIndex + 1, inlineeStatement);
                    afterStatementIndex++;
                }

                insertInlineeBlocks = false;
            }

            if (insertInlineeBlocks)
            {
                // Split block after statement being inlined
                var bottomBlock = SplitBlockAfterStatement(methodInfo.Block, afterStatementIndex);

                var blockIndex = _blocks!.IndexOf(methodInfo.Block) + 1;
                _blocks.Insert(blockIndex, bottomBlock);

                // Insert the blocks between the current block and the bottom block
                foreach (var block in blocks)
                {
                    _blocks.Insert(blockIndex++, block);

                    // For the first/entry block in the blocks being inserted
                    // link it up to the original method block
                    if (block.Predecessors.Count == 0)
                    {
                        block.Predecessors.Add(methodInfo.Block);
                        methodInfo.Block.Successors.Add(block);
                    }

                    // For exit blocks, link them to the bottom block
                    if (block.Successors.Count == 0)
                    {
                        block.Successors.Add(bottomBlock);
                        bottomBlock.Predecessors.Add(block);
                    }

                    if (block.JumpKind == JumpKind.Return)
                    {
                        // If the block ends with a return, we need to ensure that the
                        // inlinee block does not have a return statement.
                        block.JumpKind = JumpKind.Always;
                    }
                }
            }

            methodInfo.Statement.RootNode = new NothingEntry();

            return true;
        }

        public static BasicBlock SplitBlockAfterStatement(BasicBlock block, int statementIndex)
        {
            // Create a new block
            var statement = block.Statements[statementIndex + 1];
            var newBlock = new BasicBlock(statement.StartOffset);
            newBlock.JumpKind = block.JumpKind;
            block.JumpKind = JumpKind.Always; // Set the original block to always jump to the new block

            newBlock.TryStart = block.TryStart;
            newBlock.FilterStart = block.FilterStart;
            newBlock.HandlerStart = block.HandlerStart;
            newBlock.TryBlocks = block.TryBlocks.ToList();
            newBlock.Handlers = block.Handlers.ToList();

            // Move statements after the specified index to the new block
            while (block.Statements.Count > statementIndex + 1)
            {
                newBlock.Statements.Add(block.Statements[statementIndex + 1]);
                block.Statements.RemoveAt(statementIndex + 1);
            }

            foreach (var successor in block.Successors)
            {
                newBlock.Successors.Add(successor);

                successor.Predecessors.Remove(block);
                successor.Predecessors.Add(newBlock);
            }

            block.Successors.Clear();

            return newBlock;
        }

        private void PrependStatements(InlineMethodInfo methodInfo, InlineInfo inlineInfo, ref int afterStatementIndex)
        {
            // TODO: check if nullcheck required for this pointer

            int returnBufferArgIndex = -1;
            if (methodInfo.Call.HasReturnBuffer)
            {
                var hasThis = methodInfo.Call.Method!.HasThis;
                returnBufferArgIndex = hasThis ? 1 : 0;
            }

            int inlineArgumentIndex = 0;
            var arguments = methodInfo.Call.Arguments;
            for (int argumentIndex = 0; argumentIndex < arguments.Count; argumentIndex++)
            {
                if (returnBufferArgIndex >= 0 && argumentIndex == returnBufferArgIndex)
                {
                    // Skip the return buffer argument
                    continue;
                }

                InsertInlineeArgument(methodInfo, ref afterStatementIndex, inlineArgumentIndex++, arguments[argumentIndex], inlineInfo);
            }

            // Add cctor check if necessary
            if (inlineInfo.StaticConstructorMethod is not null)
            {
                // Generate call to static constructor
                var targetMethod = _nameMangler.GetMangledMethodName(inlineInfo.StaticConstructorMethod);
                var staticInitCall = new CallEntry(targetMethod, [], VarType.Void, 0);

                var newStatement = new Statement(staticInitCall);

                methodInfo.Block.Statements.Insert(afterStatementIndex + 1, newStatement);
                afterStatementIndex++;
            }

            // TODO: Add nullcheck for this pointer here

            // TODO: Zero init inlinee locals
        }

        private static void InsertInlineeArgument(InlineMethodInfo methodInfo, ref int afterStatementIndex, int argumentIndex, StackEntry argument, InlineInfo inlineInfo)
        {
            if (!inlineInfo.InlineArgumentInfos[argumentIndex].HasTemp)
                return;

            var tempNumber = inlineInfo.InlineArgumentInfos[argumentIndex].TempNumber;

            // No need for PutArgType when inlined
            if (argument is PutArgTypeEntry putArgType)
            {
                argument = putArgType.Op1;
            }

            var store = new StoreLocalVariableEntry(tempNumber, false, argument.Duplicate());

            var storeStatement = new Statement(store);

            methodInfo.Block.Statements.Insert(afterStatementIndex + 1, storeStatement);
            afterStatementIndex++;
        }
    }
}
