using dnlib.DotNet;

namespace ILCompiler.Interfaces
{
    public interface INameMangler
    {
        public string GetMangledMethodName(MethodDef method);
    }
}
