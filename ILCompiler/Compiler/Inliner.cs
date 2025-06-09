using ILCompiler.Compiler.EvaluationStack;
using ILCompiler.Interfaces;
using Microsoft.Extensions.Logging;

namespace ILCompiler.Compiler
{
    public record InlineMethod
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
    }

    public record LocalVariableInfo
    {
        public int TempNumber { get; set; }
        public bool HasTemp { get; set; }

        public VarType Type { get; set; }
    }

    public record InlineInfo
    {
        public InlineArgumentInfo[] InlineArgumentInfos { get; set; } = [];
        public LocalVariableInfo[] LocalVariableInfos { get; set; } = [];
        public required LocalVariableTable InlineLocalVariableTable { get; set; }

        public InlineCandidateInfo? InlineCandidateInfo { get; set; } = null;
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
            WalkTree(new Edge<StackEntry>(() => statement.RootNode, x => { }), null);

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

        private static void UpdateInlineReturnExpressionPlaceHolder(Edge<StackEntry> use, StackEntry? user)
        {
            while (use.Get() is ReturnExpressionEntry)
            {
                var tree = use.Get();
                var inlineCandidate = tree;

                do
                {
                    var returnExpression = inlineCandidate as ReturnExpressionEntry;
                    inlineCandidate = returnExpression!.SubstExpr;
                } while (inlineCandidate is ReturnExpressionEntry);

                use.Set(inlineCandidate!);
            }
        }
    }

    public class Inliner(ILogger<MethodCompiler> logger, IConfiguration configuration, IPhaseFactory phaseFactory) : IInliner
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<MethodCompiler> _logger = logger;
        private readonly IPhaseFactory _phaseFactory = phaseFactory;
        private string? _inputFilePath;

        public void Inline(IList<BasicBlock> blocks, LocalVariableTable locals, string inputFilePath)
        {
            _inputFilePath = inputFilePath;


            var walker = new SubstitutePlaceholdersWalker();

            foreach (var block in blocks)
            {
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
                            var inlineMethod = new InlineMethod()
                            {
                                Call = call,
                                Statement = statement,
                                StatementIndex = statementIndex,
                                Block = block,
                                Locals = locals,
                            };
                            callInlined = MorphCallInline(inlineMethod);
                        }

                        if (!callInlined)
                        {
                            statementIndex++;
                        }
                    } while (statementIndex < block.Statements.Count);
                }
            }
        }

        private bool MorphCallInline(InlineMethod method)
        {
            if (method.Call.Method == null)
            {
                throw new InvalidOperationException("Method is null");
            }

            InlineInfo inlineInfo = InitVars(method);
            inlineInfo.InlineCandidateInfo = method.Call.InlineCandidateInfo;

            var startVars = method.Locals.Count;

            var compiler = new MethodCompiler(_logger, _configuration, _phaseFactory);
            var basicBlocks = compiler.CompileInlineeMethod(method.Call.Method, _inputFilePath!, inlineInfo);

            if (basicBlocks != null)
            {
                var inlineSucceeded = InsertInlineeBlocks(basicBlocks, method, inlineInfo);
                if (inlineSucceeded)
                    return true;

                if (method.Call.Method.HasReturnType)
                {
                    inlineInfo.InlineCandidateInfo!.ReturnExpressionEntry!.SubstExpr = method.Call;

                    // Need to blank out the original call node
                    method.Statement!.RootNode = new NothingEntry();
                }

                // Need to undo some changes made during the inlining attempt
                // Temps may have been allocated need to do this
                method.Locals.ResetCount(startVars);
            }

            return false;
        }

        private static InlineInfo InitVars(InlineMethod method)
        {
            var inlineInfo = new InlineInfo
            {
                InlineLocalVariableTable = method.Locals,
                InlineArgumentInfos = new InlineArgumentInfo[method.Call.Arguments.Count]
            };

            for (int i = 0; i < method.Call.Arguments.Count; i++)
            {
                var inlineArgumentInfo = new InlineArgumentInfo()
                {
                    Argument = method.Call.Arguments[i],
                };
                inlineInfo.InlineArgumentInfos[i] = inlineArgumentInfo;
            }

            inlineInfo.LocalVariableInfos = new LocalVariableInfo[method.Call.Method!.Locals.Count];

            for (int i = 0; i < method.Call.Method!.Locals.Count; i++)
            {
                var local = method.Call.Method!.Locals[i];
                var localVariableInfo = new LocalVariableInfo()
                {
                    Type = local.Type.VarType,
                };
                inlineInfo.LocalVariableInfos[i] = localVariableInfo;
            }

            return inlineInfo;
        }

        private static bool InsertInlineeBlocks(IList<BasicBlock> blocks, InlineMethod method, InlineInfo inlineInfo)
        {
            if (blocks.Count == 1)
            {
                method.Block.Statements.RemoveAt(method.StatementIndex);

                // Prepend statements
                int afterStatementIndex = method.StatementIndex;
                PrependStatements(method, inlineInfo, ref afterStatementIndex);

                var inlineeBlock = blocks[0];
                foreach (var inlineeStatement in inlineeBlock.Statements)
                {
                    method.Block.Statements.Insert(afterStatementIndex++, inlineeStatement);
                }

                return true;
            }

            return false;
        }

        private static void PrependStatements(InlineMethod method, InlineInfo inlineInfo, ref int afterStatementIndex)
        {
            for (int argumentIndex = 0; argumentIndex < method.Call.Arguments.Count;  argumentIndex++)
            {
                InsertInlineeArgument(method, ref afterStatementIndex, argumentIndex, method.Call.Arguments[argumentIndex], inlineInfo);
            }
        }

        private static void InsertInlineeArgument(InlineMethod method, ref int afterStatementIndex, int argumentIndex, StackEntry argument, InlineInfo inlineInfo)
        {
            var tempNumber = inlineInfo.InlineArgumentInfos[argumentIndex].TempNumber;

            var store = new StoreLocalVariableEntry(tempNumber, false, argument.Duplicate());

            var storeStatement = new Statement(store);

            method.Block.Statements.Insert(afterStatementIndex++, storeStatement);
        }
    }
}
