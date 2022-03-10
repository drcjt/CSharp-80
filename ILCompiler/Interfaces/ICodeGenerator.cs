using ILCompiler.Compiler;
using ILCompiler.Compiler.DependencyAnalysis;
using Z80Assembler;

namespace ILCompiler.Interfaces
{
    public interface ICodeGenerator : IPhase
    {
        public IList<Instruction> Generate(IList<BasicBlock> blocks, IList<LocalVariableDescriptor> localVariableTable, Z80MethodCodeNode methodCodeNode);
    }
}
