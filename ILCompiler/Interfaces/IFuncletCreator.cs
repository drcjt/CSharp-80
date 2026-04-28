using ILCompiler.Compiler;
using ILCompiler.Compiler.DependencyAnalysis;

namespace ILCompiler.Interfaces
{
    internal interface IFuncletCreator : IPhase
    {
        IList<FuncletInfoDescriptor> CreateFunclets(MethodCompiler compiler, Z80MethodCodeNode method);
    }
}
