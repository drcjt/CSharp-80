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
    }

    public class Inliner : IInliner
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MethodCompiler> _logger;
        private readonly IPhaseFactory _phaseFactory;
        private string? _inputFilePath;

        public Inliner(ILogger<MethodCompiler> logger, IConfiguration configuration, IPhaseFactory phaseFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _phaseFactory = phaseFactory;
        }

        public void Inline(IList<BasicBlock> blocks, string inputFilePath)
        {
            _inputFilePath = inputFilePath;

            var block = blocks[0];

            do
            {
                if (block.Statements.Count > 0)
                {
                    int statementIndex = 0;
                    do
                    {
                        var statement = block.Statements[statementIndex];
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
                            };
                            callInlined = MorphCallInline(inlineMethod);
                        }

                        if (!callInlined)
                        {
                            statementIndex++;
                        }
                    } while (statementIndex < block.Statements.Count);
                }

                block = block.Next;
            } while (block is not null);
        }

        private bool MorphCallInline(InlineMethod method)
        {
            if (method.Call.Method == null)
            {
                throw new InvalidOperationException("Method is null");
            }

            // No support for inlining methods with return type and/or parameters yet
            if (!method.Call.Method.HasReturnType && method.Call.Method.Parameters.Count != 0)
            {
                return false;
            }

            var compiler = new MethodCompiler(_logger, _configuration, _phaseFactory);
            var basicBlocks = compiler.CompileInlineeMethod(method.Call.Method, _inputFilePath!);

            if (basicBlocks != null)
            {
                return InsertInlineeBlocks(basicBlocks, method);
            }

            return false;
        }

        private static bool InsertInlineeBlocks(IList<BasicBlock> blocks, InlineMethod method)
        {
            if (blocks.Count == 1)
            {
                method.Block.Statements.RemoveAt(method.StatementIndex);

                var inlineeBlock = blocks[0];
                foreach (var inlineeStatement in inlineeBlock.Statements)
                {
                    method.Block.Statements.Insert(method.StatementIndex++, inlineeStatement);
                }

                return true;
            }

            return false;
        }
    }
}
