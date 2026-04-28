using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler
{
    public enum FuncletKind
    {
        Root,
        Handler,
        Filter
    }

    public class FuncletInfoDescriptor
    {
        public FuncletKind Kind { get; init; }
        public IList<BasicBlock> Blocks { get; init; } = [];
    }

    public class FuncletCreator : IFuncletCreator
    {
        /// <summary>
        /// Create funclets
        /// </summary>
        /// <param name="compiler">the method compiler</param>
        /// <param name="method">the method code being compiled</param>
        public IList<FuncletInfoDescriptor> CreateFunclets(MethodCompiler compiler, Z80MethodCodeNode method)
        {
            // Create funclet list with initial root funclet
            List<FuncletInfoDescriptor> funclets =
            [
                new FuncletInfoDescriptor { Kind = FuncletKind.Root, Blocks = GetRootFuncletBlocks(compiler.ControlFlowGraph) }
            ];

            // Iterate over EH clauses and create funclets for each handler
            foreach (EHClause ehClause in compiler.ControlFlowGraph.EhClauses)
            {
                FuncletKind funcletKind = ehClause.Kind switch
                {
                    EHClauseKind.Typed => FuncletKind.Handler,
                    EHClauseKind.Finally => FuncletKind.Handler,
                    EHClauseKind.Fault => FuncletKind.Handler,
                    EHClauseKind.Filter => FuncletKind.Filter,
                    _ => throw new InvalidOperationException($"Unexpected EH clause kind: {ehClause.Kind}")
                };
                var handlerBlocks = compiler.ControlFlowGraph.Blocks.Where(b => compiler.ControlFlowGraph.IsInHandlerRegion(b, ehClause)).ToList();
                funclets.Add(new FuncletInfoDescriptor
                {
                    Kind = funcletKind,
                    Blocks = handlerBlocks
                });
            }

            return funclets;
        }

        public static IList<BasicBlock> GetRootFuncletBlocks(FlowGraph cfg)
        {
            var handlerBlocks = new List<BasicBlock>();
            foreach (var clause in cfg.EhClauses)
            {
                foreach (var block in cfg.Blocks)
                {
                    if (cfg.IsInHandlerRegion(block, clause))
                    {
                        handlerBlocks.Add(block);
                    }
                }
            }

            return cfg.Blocks.Where(b => !handlerBlocks.Contains(b)).ToList();
        }
    }
}
