namespace ILCompiler.Common.TypeSystem.Common
{
    public class ParameterizedType : TypeDesc
    {
        public TypeDesc ParameterType { get; init; }
        public ParameterizedType(TypeDesc parameterType)
        {
            ParameterType = parameterType;
        }

        public override TypeSystemContext Context => ParameterType.Context;
    }
}
