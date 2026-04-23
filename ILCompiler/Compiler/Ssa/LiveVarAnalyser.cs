using ILCompiler.Compiler.FlowgraphHelpers;

namespace ILCompiler.Compiler.Ssa
{
    internal sealed class LiveVarAnalyzer
    {
        readonly VariableSet _liveIn;
        readonly VariableSet _liveOut;
        readonly FlowGraph _controlFlowGraph;

        public static void AnalyzeLiveVars(FlowGraph controlFlowGraph, FlowgraphDfsTree dfsTree)
        {
            var analyzer = new LiveVarAnalyzer(controlFlowGraph);
            analyzer.Run(dfsTree);
        }

        public LiveVarAnalyzer(FlowGraph controlFlowGraph)
        {
            _liveIn = new VariableSet();
            _liveOut = new VariableSet();
            _controlFlowGraph = controlFlowGraph;
        }

        private bool PerBlockAnalysis(BasicBlock block)
        {
            // Compute the liveOut set
            _liveOut.Clear();

            // When block ends with a Jmp need to mark all parameters as live at the Jmp
            // Note importer does not currently handle jmp cil and the JumpEntry node is
            // only generated from importing br and br.s so no need to worry about this
            // for now.

            // TODO: Revisit when importer handles jmp cil

            // Union in all the live in vars of blocks successors
            foreach (var succ in _controlFlowGraph.VisitAllSuccs(block))
            {
                _liveOut.Union(succ.LiveIn);
            }

            // Compute the liveIn set
            // liveIn = use | (out & ~def)
            Liveness(_liveIn, block.VarDef, block.VarUse, _liveOut);

            // Update the blocks LiveIn and LiveOut if there are any changes
            bool liveInChanged = !block.LiveIn.Equals(_liveIn);
            bool liveOutChanged = !block.LiveOut.Equals(_liveOut);
            if (liveInChanged || liveOutChanged)
            {
                block.LiveIn.Assign(_liveIn);
                block.LiveOut.Assign(_liveOut);
            }

            return liveInChanged;
        }

        // Compute the live_in set. Variable is alive if there is use or it is out set, but not in def.
        // in = use | (out & ~def)
        public static void Liveness(VariableSet liveIn, VariableSet def, VariableSet use, VariableSet liveOut)
        {
            liveIn.Clear();
            foreach (var variable in use)
            {
                liveIn.AddElem(variable);
            }

            foreach (var variable in liveOut)
            {
                if (!def.IsMember(variable))
                {
                    liveIn.AddElem(variable);
                }
            }
        }

        public void Run(FlowgraphDfsTree dfsTree)
        {
            bool changed;
            do
            {
                changed = false;

                _liveIn.Clear();
                _liveOut.Clear();

                foreach (BasicBlock block in dfsTree.PostOrder)
                {
                    if (PerBlockAnalysis(block))
                    {
                        changed = true;
                    }
                }
            } while (changed && dfsTree.HasCycle);
        }
    }
}
