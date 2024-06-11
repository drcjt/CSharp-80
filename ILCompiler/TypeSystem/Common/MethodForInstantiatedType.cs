using dnlib.DotNet;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.TypeSystem.Common
{
    internal class MethodForInstantiatedType : MethodDesc
    {
        private readonly MethodDesc _typicalMethodDef;
        private readonly InstantiatedType _instantiatedType;

        public MethodForInstantiatedType(MethodDesc typicalMethodDef, InstantiatedType instantiatedType)
        {
            _typicalMethodDef = typicalMethodDef;
            _instantiatedType = instantiatedType;
        }

        public override TypeSystemContext Context => _typicalMethodDef.Context;

        public override TypeDesc OwningType => _instantiatedType;

        public override MethodSignature Signature
        {
            get
            {
                var template = _typicalMethodDef.Signature;
                var builder = new MethodSignatureBuilder(template);

                builder.ReturnType = Instantiate(template.ReturnType);
                for (int i = 0; i < template.Length; i++)
                {
                    builder[i] = Instantiate(template[i].Type);
                }

                return builder.ToSignature();
            }
        }

        public override IList<MethodParameter> Parameters => throw new NotImplementedException();

        public override IList<LocalVariableDefinition> Locals => throw new NotImplementedException();

        public override MethodIL? MethodIL => throw new NotImplementedException();

        public override IList<MethodOverride> Overrides => throw new NotImplementedException();

        public override MethodSig MethodSig => throw new NotImplementedException();

        public override CustomAttributeCollection CustomAttributes => throw new NotImplementedException();

        public override Instantiation Instantiation => throw new NotImplementedException();

        public override bool HasCustomAttribute(string attributeNamespace, string attributeName)
        {
            throw new NotImplementedException();
        }

        private TypeDesc Instantiate(TypeDesc type)
        {
            return type.InstantiateSignature(_instantiatedType.Instantiation, null);
        }
    }
}
