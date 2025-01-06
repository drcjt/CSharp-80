using ILCompiler.Interfaces;
using ILCompiler.TypeSystem.Common;

namespace ILCompiler.Compiler
{
    public class NameMangler : INameMangler
    {
        private int nextMethodId = 0;
        private readonly Dictionary<MethodDesc, String> _mangledMethodNames = new Dictionary<MethodDesc, String>();

        private int nextFieldId = 0;
        private readonly Dictionary<FieldDesc, String> _mangledFieldNames = new Dictionary<FieldDesc, String>();

        private int nextTypeId = 0;
        private readonly Dictionary<TypeDesc, String> _mangledTypeNames = new Dictionary<TypeDesc, String>();

        public string GetMangledMethodName(MethodDesc method)
        {
            if (_mangledMethodNames.TryGetValue(method, out string? mangledName))
            {
                return mangledName;
            }

            mangledName = $"m{nextMethodId++}";
            _mangledMethodNames.Add(method, mangledName);

            return mangledName;
        }

        public string GetMangledFieldName(FieldDesc field)
        {
            if (_mangledFieldNames.TryGetValue(field, out string? mangledName))
            {
                return mangledName;
            }

            mangledName = $"f{nextFieldId++}";
            _mangledFieldNames.Add(field, mangledName);

            return mangledName;
        }

        public string GetMangledTypeName(TypeDesc type)
        {
            if (_mangledTypeNames.TryGetValue(type, out string? mangledName))
            {
                return mangledName;
            }

            mangledName = $"t{nextTypeId++}";
            _mangledTypeNames.Add(type, mangledName);

            return mangledName;
        }

        public string GetUniqueName()
        {
            return $"u{nextFieldId++}";
        }
    }
}