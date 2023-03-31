using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.Emit;
using ILCompiler.Interfaces;

namespace ILCompiler.Compiler.CodeGenerators
{
    public class CodeGeneratorContext
    {
        public Emitter Emitter { get; } = new();
        public IList<LocalVariableDescriptor> LocalVariableTable { get; }
        public int ParamsCount => _method.ParamsCount;
        public int LocalsCount => _method.LocalsCount;
        public MethodDesc Method => _method.Method;

        public INameMangler NameMangler { get; }

        private readonly Z80MethodCodeNode _method;

        public readonly IConfiguration Configuration;

        public CodeGeneratorContext(IList<LocalVariableDescriptor> localVariableTable, Z80MethodCodeNode method, IConfiguration configuration, INameMangler nameMangler)
        {
            LocalVariableTable = localVariableTable;
            _method = method;
            Configuration = configuration;
            NameMangler = nameMangler;
        }
    }
}
