using dnlib.DotNet;
using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler.PreInit
{
    public class PreinitializationManager
    {
        private readonly IDictionary<TypeDef, PreinitializationInfo> _preInitializationInfoByType = new Dictionary<TypeDef, PreinitializationInfo>();

        public bool IsPreinitialized(TypeDef type)
        {
            if (!type.HasStaticConstructor())
            {
                return false;
            }

            return GetPreinitializationInfo(type).IsPreinitialized;
        }

        public PreinitializationInfo GetPreinitializationInfo(TypeDef type)
        {
            return new PreinitializationInfo();
        }
    }
}
