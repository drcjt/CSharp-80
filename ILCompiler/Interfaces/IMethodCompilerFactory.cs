using dnlib.DotNet;

namespace ILCompiler.Interfaces
{
    public interface IMethodCompilerFactory
    {
        public IMethodCompiler Create(MethodDef method);
    }
}
