using dnlib.DotNet;
using ILCompiler.Compiler.DependencyAnalysis;
using System.Collections.Generic;
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

        public CodeGeneratorContext(IList<LocalVariableDescriptor> localVariableTable, Z80MethodCodeNode method)
        {
            LocalVariableTable = localVariableTable;
            _method = method;
        }
    }
}
