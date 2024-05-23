namespace ILCompiler.Common.TypeSystem.Common
{
    public class MethodSignatureBuilder
    {
        private MethodSignature _template;
        private TypeDesc _returnType;
        private readonly MethodParameter[] _parameters;

        public MethodSignatureBuilder(MethodSignature template)
        {
            _template = template;
            _returnType = template.ReturnType;

            _parameters = new MethodParameter[template.Parameters.Length];
            for (int i = 0; i < template.Parameters.Length; i++)
            {
                _parameters[i] = template.Parameters[i];
            }
        }

        public MethodSignature ToSignature()
        {
            _template = new MethodSignature(_template.IsStatic, _returnType, _parameters);
            return _template;
        }

        public TypeDesc ReturnType 
        { 
            set 
            { 
                _returnType = value; 
            } 
        }

        public TypeDesc this[int index] 
        {
            set
            {
                _parameters[index].Type = value;
            }
        }
    }
}
