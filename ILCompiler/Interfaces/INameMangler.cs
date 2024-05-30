using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.Common;

namespace ILCompiler.Interfaces
{
    public interface INameMangler
    {
        public string GetMangledMethodName(MethodDef method);
        public string GetMangledMethodName(MethodDesc method);
        public string GetMangledFieldName(FieldDesc field);
        public string GetMangledTypeName(TypeDesc type);
        public string GetUniqueName();
    }
}
