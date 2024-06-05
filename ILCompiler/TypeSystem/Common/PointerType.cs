using ILCompiler.Compiler;

namespace ILCompiler.TypeSystem.Common
{
    public class PointerType : ParameterizedType
    {
        public PointerType(TypeDesc parameterType) : base(parameterType)
        {
        }

        public override VarType VarType => VarType.Ptr;

        public override TypeDesc InstantiateSignature(Instantiation? typeInstantiation, Instantiation methodInstantiation)
        {
            var instantiatedParameterType = ParameterType.InstantiateSignature(typeInstantiation, methodInstantiation);
            if (instantiatedParameterType != ParameterType)
                return Context.GetPointerType(instantiatedParameterType);

            return this;
        }
    }
}
