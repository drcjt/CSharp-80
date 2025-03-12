using ILCompiler.TypeSystem.Common;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Interfaces;
using ILCompiler.Compiler.Emit;

namespace ILCompiler.Compiler.CodeGenerators
{
    public class CodeGeneratorContext
    {
        public InstructionsBuilder InstructionsBuilder { get; } = new();
        public LocalVariableTable LocalVariableTable { get; }
        public int LocalsCount => _method.LocalsCount;
        public MethodDesc Method => _method.Method;

        public bool GeneratedEpilog { get; set; }

        public INameMangler NameMangler { get; }
        public NodeFactory NodeFactory { get; }

        private readonly Z80MethodCodeNode _method;

        public readonly IConfiguration Configuration;
        public readonly CorLibModuleProvider CorLibModuleProvider;

        public CodeGeneratorContext(LocalVariableTable localVariableTable, Z80MethodCodeNode method, IConfiguration configuration, INameMangler nameMangler, NodeFactory nodeFactory, CorLibModuleProvider corLibModuleProvider)
        {
            LocalVariableTable = localVariableTable;
            _method = method;
            Configuration = configuration;
            NameMangler = nameMangler;
            NodeFactory = nodeFactory;
            CorLibModuleProvider = corLibModuleProvider;
        }
    }
}
