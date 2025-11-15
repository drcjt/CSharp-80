using ILCompiler.Compiler;
using ILCompiler.Compiler.DependencyAnalysis;
using ILCompiler.Compiler.Emit;

namespace ILCompiler.Interfaces
{
    public interface ICodeGenerator : IPhase
    {
        public IList<Instruction> Generate(MethodCompiler compiler, Z80MethodCodeNode methodCodeNode);
    }
}
