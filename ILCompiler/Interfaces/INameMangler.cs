using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.Common;

namespace ILCompiler.Interfaces
{
    public interface INameMangler
    {
        public string GetMangledMethodName(MethodSpec calleemethod, MethodDesc callerMethod);
        public string GetMangledMethodName(MethodDef method);
        public string GetMangledMethodName(MethodDesc method);
        public string GetMangledFieldName(FieldDef field);
        public string GetUniqueName();
    }
}
