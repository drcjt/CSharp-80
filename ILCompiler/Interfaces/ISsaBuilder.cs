using ILCompiler.Compiler;

namespace ILCompiler.Interfaces
{
    public interface ISsaBuilder : IPhase
    {
        public void Build(MethodCompiler compiler);
    }
}
