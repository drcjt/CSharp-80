using dnlib.DotNet;

namespace ILCompiler.Compiler.PreInit
{
    public class PreinitializationInfo
    {
        private readonly Dictionary<FieldDef, ISerializableValue> _fieldValues = new Dictionary<FieldDef, ISerializableValue>();

        public bool IsPreinitialized => _fieldValues.Count > 0;
    }
}
