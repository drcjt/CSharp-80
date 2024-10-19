using ILCompiler.Compiler;
using ILCompiler.TypeSystem.Canon;

namespace ILCompiler.TypeSystem.Common
{
    public class PointerType : ParameterizedType
    {
        public PointerType(TypeDesc parameterType) : base(parameterType)
        {
        }

        public override VarType VarType => VarType.Ptr;

        public override TypeDesc InstantiateSignature(Instantiation? typeInstantiation, Instantiation? methodInstantiation)
        {
            var instantiatedParameterType = ParameterType.InstantiateSignature(typeInstantiation, methodInstantiation);
            if (instantiatedParameterType != ParameterType)
                return Context.GetPointerType(instantiatedParameterType);

            return this;
        }

        protected override TypeDesc ConvertToCanonFormImpl(CanonicalFormKind kind)
        {
            TypeDesc paramTypeConverted = Context.ConvertToCanon(ParameterType, kind);
            if (paramTypeConverted != ParameterType)
                return Context.GetPointerType(paramTypeConverted);

            return this;
        }
    }
}
