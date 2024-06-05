using ILCompiler.Compiler;

namespace ILCompiler.TypeSystem.Common
{
    internal class ByRefType : ParameterizedType
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
    }
}
