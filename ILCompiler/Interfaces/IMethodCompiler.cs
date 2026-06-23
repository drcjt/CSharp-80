using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.OpcodeImporters;

namespace ILCompiler.Interfaces
{
    public interface IMethodCompiler
    {
        public CodeFolder CodeFolder { get; }
        public void CompileMethod(Z80MethodCodeNode methodCodeNodeNeedingCode, string inputFilePath);
    }
}
