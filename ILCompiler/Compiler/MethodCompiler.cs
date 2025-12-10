using System.Text;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.Dominators;
using ILCompiler.Compiler.FlowgraphHelpers;
using ILCompiler.Compiler.Inlining;
using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;
using Microsoft.Extensions.Logging;

namespace ILCompiler.Compiler
{
    public class MethodCompiler : IMethodCompiler
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MethodCompiler> _logger;
        private readonly IPhaseFactory _phaseFactory;


        public MethodDesc? Method { get; private set; }
        public LocalVariableTable Locals { get; } = [];
        public FlowgraphDfsTree? DfsTree { get; private set; }
        public FlowgraphDominatorTree? DominatorTree { get; private set; }
        public FlowGraphNaturalLoops? Loops { get; private set; }
        public IFlowgraph? Flowgraph { get; private set; }
        public IList<BasicBlock> Blocks { get; } = [];

        public MethodCompiler(ILogger<MethodCompiler> logger, IConfiguration configuration, IPhaseFactory phaseFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _phaseFactory = phaseFactory;
        }

        public IList<BasicBlock>? CompileInlineeMethod(MethodDesc method, string inputFilePath, InlineInfo inlineInfo)
        {
            _logger.LogDebug("Compiling inlinee method {MethodName}", method.Name);

            if (method.HasCustomAttribute("System.Runtime", "RuntimeImportAttribute"))
            {
                return null;
            }
            if (method.IsPInvoke || method.IsInternalCall)
            {
                return null;
            }

            if (method.IsIntrinsic && method.MethodIL == null)
            {
                // Deal with intrinsics handled by code gen so no need to import
                return null;
            }

            Method = method;

            Locals.SetupLocalVariableTable(Method);

            var ilImporter = _phaseFactory.Create<IImporter>();

            // When inlining we only run the import phase
            IList<EHClause> ehClauses = new List<EHClause>();
            ilImporter.Import(this, ehClauses, inlineInfo);

            if (_configuration.DumpFlowGraphs)
            {
                Diagnostics.DumpFlowGraph(inputFilePath, method, Blocks);
            }

            return Blocks;
        }

        public void CompileMethod(Z80MethodCodeNode methodCodeNodeNeedingCode, string inputFilePath)
        {
            var method = methodCodeNodeNeedingCode.Method;

            _logger.LogDebug("Compiling method {MethodName}", method.Name);

            if (method.HasCustomAttribute("System.Runtime", "RuntimeImportAttribute"))
            {
                return;
            }
            if (method.IsPInvoke || method.IsInternalCall)
            {
                return;
            }

            if (method.IsIntrinsic && method.MethodIL == null)
            {
                // Deal with intrinsics handled by code gen so no need to import
                return;
            }

            Method = method;

            Locals.SetupLocalVariableTable(Method);

            var ilImporter = _phaseFactory.Create<IImporter>();

            // Main phases of the compiler live here
            ilImporter.Import(this, methodCodeNodeNeedingCode.EhClauses);

            if (_configuration.DumpFlowGraphs)
            {
                Diagnostics.DumpFlowGraph(inputFilePath, method, Blocks);
            }
            DumpIRTrees("After Import");

            var morpher = _phaseFactory.Create<IMorpher>();
            morpher.Init(this);

            // Inlining
            var inliner = _phaseFactory.Create<IInliner>();
            inliner.Inline(this, inputFilePath);
            DumpIRTrees("After Inlining");

            morpher.Morph();
            DumpIRTrees("After Morph");

            if (_configuration.Optimize)
            {
                // Build the dfs tree and remove unreachable blocks
                DfsTree = FlowgraphDfsTree.BuildAndRemove(Blocks);
            }

            Flowgraph = _phaseFactory.Create<IFlowgraph>();
            Flowgraph.SetBlockOrder(Blocks);

            if (_configuration.Optimize)
            {
                var flowgraphDominatorTreeBuilder = _phaseFactory.Create<IComputeDominators>();
                DominatorTree = flowgraphDominatorTreeBuilder.Build(this);

                var ssaBuilder = _phaseFactory.Create<ISsaBuilder>();
                ssaBuilder.Build(this);

                // Find loops
                var loopFinder = _phaseFactory.Create<ILoopFinder>();
                Loops = loopFinder.FindLoops(this);

                // Optimize induction variables
                var inductionVarOptimizer = _phaseFactory.Create<IInductionVariableOptimizer>();
                inductionVarOptimizer.Run(this);

                DumpIRTrees("After Strength Reduction");

                // Early Value Propagation
                var earlyValuePropagation = _phaseFactory.Create<IEarlyValuePropagation>();
                earlyValuePropagation.Run(this);
            }

            // Rationalize
            // LIR valid from here on - nodes are fully linked across statements
            var rationalizer = _phaseFactory.Create<IRationalizer>();
            rationalizer.Rationalize(this);

            DumpLIRTrees();

            // Lower
            var lowering = _phaseFactory.Create<ILowering>();
            lowering.Run(this);

            var codeGenerator = _phaseFactory.Create<ICodeGenerator>();
            var instructions = codeGenerator.Generate(this, methodCodeNodeNeedingCode);
            methodCodeNodeNeedingCode.MethodCode = instructions;
        }

        private void DumpLIRTrees()
        {
            if (_configuration.DumpIRTrees)
            {
                // Dump LIR here
                var lirDumper = new LIRDumper();
                var lirDump = lirDumper.Dump(Blocks);
                _logger.LogInformation("{LirDump}", lirDump);
            }
        }

        private void DumpIRTrees(string message)
        {
            if (_configuration.DumpIRTrees)
            {
                _logger.LogInformation(message);
                _logger.LogInformation("METHOD: {MethodFullName}", Method!.FullName);

                int lclNum = 0;
                StringBuilder sb = new();
                sb.AppendLine("Local var info:");
                foreach (var lclVar in Locals)
                {
                    var name = lclVar.Name;
                    if (lclVar.IsTemp)
                    {
                        name = "tmp";
                    }

                    sb.AppendLine($"  V{lclNum:00} {name} {lclVar.IsParameter} {lclVar.Type.ToString().ToLower()} {lclVar.ExactSize}");

                    lclNum++;
                }
                _logger.LogInformation("{LocalVars}", sb.ToString());

                var treeDumper = new TreeDumper();
                var treedump = treeDumper.Dump(Blocks, Locals);
                _logger.LogInformation("{Treedump}", treedump);
            }
        }
    }
}
