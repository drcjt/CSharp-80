using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Interfaces;
using ILCompiler.Compiler.Emit;

namespace ILCompiler.Compiler.CodeGenerators
{
    public class CodeGeneratorContext
    {
        public Emitter Emitter { get; } = new();
        public LocalVariableTable LocalVariableTable { get; }
        public int ParamsCount => _method.ParamsCount;
        public int LocalsCount => _method.LocalsCount;
        public MethodDesc Method => _method.Method;

        public INameMangler NameMangler { get; }
        public NodeFactory NodeFactory { get; }

        private readonly Z80MethodCodeNode _method;

        public readonly IConfiguration Configuration;

        public CodeGeneratorContext(LocalVariableTable localVariableTable, Z80MethodCodeNode method, IConfiguration configuration, INameMangler nameMangler, NodeFactory nodeFactory)
        {
            LocalVariableTable = localVariableTable;
            _method = method;
            Configuration = configuration;
            NameMangler = nameMangler;
            NodeFactory = nodeFactory;
        }
    }
}
