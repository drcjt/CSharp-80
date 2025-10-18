using System.Text;
using ILCompiler.Compiler.DependencyAnalysis;
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

        private int? _returnBufferArgIndex;

        private readonly LocalVariableTable _locals;

        public MethodCompiler(ILogger<MethodCompiler> logger, IConfiguration configuration, IPhaseFactory phaseFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _locals = new LocalVariableTable();
            _phaseFactory = phaseFactory;
        }

        private int SetupLocalVariableTable(MethodDesc method)
        {
            int parameterCount = 0;

            if (method.HasThis)
            {
                var local = new LocalVariableDescriptor()
                {
                    IsParameter = true,
                    IsTemp = false,
                    Name = "",
                    ExactSize = 2,
                    Type = VarType.Ref,
                };
                _locals.Add(local);
                parameterCount++;
            }

            // Setup local variable table - includes parameters as well as locals in method
            for (int parameterIndex = 0; parameterIndex < method.Signature.Length; parameterIndex++)
            {
                var parameter = method.Signature[parameterIndex];
                var local = new LocalVariableDescriptor()
                {
                    IsParameter = true,
                    IsTemp = false,
                    Name = parameter.Name,
                    ExactSize = parameter.Type.GetElementSize().AsInt,
                    Type = parameter.Type.VarType,
                };
                _locals.Add(local);
                parameterCount++;
            }

            foreach (var local in method.Locals)
            {
                var localVariableDescriptor = new LocalVariableDescriptor()
                {
                    IsParameter = false,
                    IsTemp = false,
                    Name = local.Name,
                    ExactSize = local.Type.GetElementSize().AsInt,
                    Type = local.Type.VarType,
                };
                _locals.Add(localVariableDescriptor);
            }

            if (!method.Signature.ReturnType.IsVoid)
            {
                var returnType = method.Signature.ReturnType;
                InitReturnBufferArg(returnType, method.HasThis, ref parameterCount);
            }

            return parameterCount;
        }

        private void InitReturnBufferArg(TypeDesc returnType, bool hasThis, ref int parameterCount)
        {
            if (returnType.IsValueType && !returnType.IsPrimitive && !returnType.IsEnum)
            {
                var target = new TargetDetails(TypeSystem.Common.TargetArchitecture.Z80);

                var returnBuffer = new LocalVariableDescriptor()
                {
                    IsParameter = true,
                    Type = VarType.ByRef,
                    IsTemp = false,
                    ExactSize = target.PointerSize,
                };

                // Ensure return buffer parameter goes after the this parameter if present
                _returnBufferArgIndex = hasThis ? 1 : 0;
                _locals.Insert(_returnBufferArgIndex.Value, returnBuffer);
                parameterCount++;
            }
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

            var parameterCount = SetupLocalVariableTable(method);

            var ilImporter = _phaseFactory.Create<IImporter>();

            // When inlining we only run the import phase
            IList<EHClause> ehClauses = new List<EHClause>();
            var basicBlocks = ilImporter.Import(parameterCount, _returnBufferArgIndex, method, _locals, ehClauses, inlineInfo);

            if (_configuration.DumpFlowGraphs)
            {
                Diagnostics.DumpFlowGraph(inputFilePath, method, basicBlocks);
            }

            return basicBlocks;
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

            var parameterCount = SetupLocalVariableTable(method);

            var ilImporter = _phaseFactory.Create<IImporter>();

            // Main phases of the compiler live here
            var basicBlocks = ilImporter.Import(parameterCount, _returnBufferArgIndex, method, _locals, methodCodeNodeNeedingCode.EhClauses);

            if (_configuration.DumpFlowGraphs)
            {
                Diagnostics.DumpFlowGraph(inputFilePath, method, basicBlocks);
            }
            DumpIRTrees(method, basicBlocks, "After Import");

            var morpher = _phaseFactory.Create<IMorpher>();
            morpher.Init(method, basicBlocks);

            // Inlining
            var inliner = _phaseFactory.Create<IInliner>();
            inliner.Inline(basicBlocks, _locals, inputFilePath);
            DumpIRTrees(method, basicBlocks, "After Inlining");

            morpher.Morph(basicBlocks, _locals);
            DumpIRTrees(method, basicBlocks, "After Morph");

            FlowgraphDfsTree? dfsTree = null;
            if (_configuration.Optimize)
            {
                // Build the dfs tree and remove unreachable blocks
                dfsTree = FlowgraphDfsTree.BuildAndRemove(basicBlocks);
            }

            var flowgraph = _phaseFactory.Create<IFlowgraph>();
            flowgraph.SetBlockOrder(basicBlocks);

            if (_configuration.Optimize)
            {
                var ssaBuilder = _phaseFactory.Create<ISsaBuilder>();
                ssaBuilder.Build(basicBlocks, _locals, _configuration.DumpSsa, dfsTree!);

                // Find loops
                var loopFinder = _phaseFactory.Create<ILoopFinder>();
                var loops = loopFinder.FindLoops(basicBlocks, dfsTree!);

                // Optimize induction variables
                var inductionVarOptimizer = _phaseFactory.Create<IInductionVariableOptimizer>();
                inductionVarOptimizer.Run(basicBlocks, loops, _locals);

                // Early Value Propagation
                var earlyValuePropagation = _phaseFactory.Create<IEarlyValuePropagation>();
                earlyValuePropagation.Run(basicBlocks, _locals);
            }

            // Rationalize
            // LIR valid from here on - nodes are fully linked across statements
            var rationalizer = _phaseFactory.Create<IRationalizer>();
            rationalizer.Rationalize(basicBlocks);

            if (_configuration.DumpIRTrees)
            {
                // Dump LIR here
                var lirDumper = new LIRDumper();
                var lirDump = lirDumper.Dump(basicBlocks);
                _logger.LogInformation("{LirDump}", lirDump);
            }

            // Lower
            var lowering = _phaseFactory.Create<ILowering>();
            lowering.Run(basicBlocks, _locals);

            var codeGenerator = _phaseFactory.Create<ICodeGenerator>();
            var instructions = codeGenerator.Generate(basicBlocks, _locals, methodCodeNodeNeedingCode);
            methodCodeNodeNeedingCode.MethodCode = instructions;
        }

        private void DumpIRTrees(MethodDesc method, IList<BasicBlock> basicBlocks, string message)
        {
            if (_configuration.DumpIRTrees)
            {
                _logger.LogInformation(message);
                _logger.LogInformation("METHOD: {MethodFullName}", method.FullName);

                int lclNum = 0;
                StringBuilder sb = new();
                foreach (var lclVar in _locals)
                {
                    sb.AppendLine($"LCLVAR {lclNum} {lclVar.Name} {lclVar.IsParameter} {lclVar.Type} {lclVar.ExactSize}");

                    lclNum++;
                }
                _logger.LogInformation("{LocalVars}", sb.ToString());

                var treeDumper = new TreeDumper();
                var treedump = treeDumper.Dump(basicBlocks);
                _logger.LogInformation("{Treedump}", treedump);
            }
        }
    }
}
