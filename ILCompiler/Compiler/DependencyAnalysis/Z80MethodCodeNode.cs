using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Compiler.DependencyAnalysisFramework;
using ILCompiler.Compiler.Emit;
using ILCompiler.Interfaces;
using ILCompiler.IoC;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    public class Z80MethodCodeNode : DependencyNode
    {
        public MethodDesc Method { get; }
        public override string Name => Method.FullName;

        private readonly Factory<IMethodCompiler> _methodCompilerFactory;

        public Z80MethodCodeNode(MethodDesc method, Factory<IMethodCompiler> methodCompilerFactory)
        {
            Method = method;
            ParamsCount = method.Parameters().Count;
            LocalsCount = method.Body?.Variables.Count ?? 0;

            _methodCompilerFactory = methodCompilerFactory;
        }

        public bool HasExceptionHandlers => Method?.Body?.HasExceptionHandlers ?? false;
        public IList<Instruction> MethodCode { get; set; } = new List<Instruction>();

        public IList<EHClause> EhClauses { get; set; } = new List<EHClause>();

        public int ParamsCount { get; set; }
        public int LocalsCount { get; set; }

        public override IList<IDependencyNode> GetStaticDependencies(DependencyNodeContext context)
        {
            var scanner = new ILScanner(Method, context);
            return scanner.FindDependencies();
        }

        private bool _compiled = false;
        public override IList<Instruction> GetInstructions(string inputFilePath) 
        {
            if (!_compiled)
            {
                var methodCompiler = _methodCompilerFactory.Create();
                methodCompiler.CompileMethod(this, inputFilePath);
                _compiled = true;
            }

            if (MethodCode.Count > 0)
            {
                MethodCode.Insert(0, Instruction.CreateComment($"Assembly listing for method {Method.FullName}"));
                MethodCode.Insert(1, Instruction.CreateComment("ix based frame"));
            }
            return MethodCode;
        }
    }
}
