using dnlib.DotNet;

namespace ILCompiler.Compiler.PreInit
{
    public class PreinitializationInfo
    {
        private readonly Dictionary<FieldDef, ISerializableValue> _fieldValues = new Dictionary<FieldDef, ISerializableValue>();

        public bool IsPreinitialized => _fieldValues.Count > 0;

        public PreinitializationInfo() 
        { 
        }

        public PreinitializationInfo(IEnumerable<KeyValuePair<FieldDef, ISerializableValue>> fieldValues)
        {
            foreach (var field in fieldValues)
            {
                _fieldValues.Add(field.Key, field.Value);
            }
        }

        public ISerializableValue GetFieldValue(FieldDef field)
        {
            return _fieldValues[field];
        }
    }
}
