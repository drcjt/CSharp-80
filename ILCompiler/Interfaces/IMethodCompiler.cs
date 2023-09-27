using ILCompiler.Compiler.DependencyAnalysis;

namespace ILCompiler.Interfaces
{
    public interface IMethodCompiler
    {
        public void CompileMethod(Z80MethodCodeNode methodCodeNodeNeedingCode, string inputFilePath);
    }
}
