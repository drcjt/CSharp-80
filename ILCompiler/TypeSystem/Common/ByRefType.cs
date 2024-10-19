using ILCompiler.Compiler;
using ILCompiler.TypeSystem.Canon;

namespace ILCompiler.TypeSystem.Common
{
    public class ByRefType : ParameterizedType
    {
        public ByRefType(TypeDesc parameter) : base(parameter)
        {
        }

        public override VarType VarType => VarType.ByRef;

        public override TypeDesc InstantiateSignature(Instantiation? typeInstantiation, Instantiation? methodInstantiation)
        {
            var parameterType = this.ParameterType;
            var instantiatedParameterType = parameterType.InstantiateSignature(typeInstantiation, methodInstantiation);

            return new ByRefType(instantiatedParameterType);
        }

        protected override TypeDesc ConvertToCanonFormImpl(CanonicalFormKind kind)
        {
            TypeDesc paramTypeConverted = Context.ConvertToCanon(ParameterType, kind);
            if (paramTypeConverted != ParameterType)
                return Context.GetByRefType(paramTypeConverted);

            return this;
        }
    }
}
