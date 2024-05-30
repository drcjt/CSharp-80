using ILCompiler.Common.TypeSystem.Common;

namespace ILCompiler.Compiler.PreInit
{
    public class PreinitializationInfo
    {
        private readonly Dictionary<FieldDesc, ISerializableValue> _fieldValues = new Dictionary<FieldDesc, ISerializableValue>();

        public bool IsPreinitialized => _fieldValues.Count > 0;

        public PreinitializationInfo() 
        { 
        }

        public PreinitializationInfo(IEnumerable<KeyValuePair<FieldDesc, ISerializableValue>> fieldValues)
        {
            foreach (var field in fieldValues)
            {
                _fieldValues.Add(field.Key, field.Value);
            }
        }

        public ISerializableValue GetFieldValue(FieldDesc field)
        {
            return _fieldValues[field];
        }
    }
}
