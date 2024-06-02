using ILCompiler.Common.TypeSystem.Common;
using ILCompiler.Common.TypeSystem.Common.Dnlib;

namespace ILCompiler.Compiler.PreInit
{
    public class PreinitializationManager
    {
        private readonly IDictionary<TypeDesc, PreinitializationInfo> _preInitializationInfoByType = new Dictionary<TypeDesc, PreinitializationInfo>();
        private readonly DnlibModule _module;
        public PreinitializationManager(DnlibModule module)
        {
            _module = module;
        }

        public bool IsPreinitialized(TypeDesc type)
        {
            if (!type.HasStaticConstructor)
            {
                return false;
            }

            return GetPreinitializationInfo(type).IsPreinitialized;
        }

        public PreinitializationInfo GetPreinitializationInfo(TypeDesc type)
        {
            if (!_preInitializationInfoByType.ContainsKey(type))
            {
                _preInitializationInfoByType[type] = TypePreinitializer.ScanType(type, _module);
            }
            return _preInitializationInfoByType[type];
        }

        public bool HasLazyStaticConstructor(TypeDesc type)
        {
            if (!type.HasStaticConstructor)
            {
                return false;
            }

            return !IsPreinitialized(type);
        }
    }
}
