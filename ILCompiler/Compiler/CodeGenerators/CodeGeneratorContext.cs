using dnlib.DotNet;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Interfaces;
using Z80Assembler;

namespace ILCompiler.Compiler.CodeGenerators
{
    public class CodeGeneratorContext
    {
        public Assembler Assembler = new Assembler();
        public IList<LocalVariableDescriptor> LocalVariableTable;
        public int ParamsCount => _method.ParamsCount;
        public int LocalsCount => _method.LocalsCount;
        public MethodDef Method => _method.Method;

        private Z80MethodCodeNode _method;

        public readonly IConfiguration Configuration;

        public CodeGeneratorContext(IList<LocalVariableDescriptor> localVariableTable, Z80MethodCodeNode method, IConfiguration configuration)
        {
            LocalVariableTable = localVariableTable;
            _method = method;
            Configuration = configuration;
        }
    }
}
