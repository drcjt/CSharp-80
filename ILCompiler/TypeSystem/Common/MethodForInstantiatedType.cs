using ILCompiler.TypeSystem.Canon;
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

        public override IList<MethodParameter> Parameters => _typicalMethodDef.Parameters;

        public override IList<LocalVariableDefinition> Locals
        {
            get
            {
                var instantiatedLocals = new List<LocalVariableDefinition>();
                foreach (var local in _typicalMethodDef.Locals)
                {
                    var instantiatedLocal = new LocalVariableDefinition(Instantiate(local.Type), local.Name, local.Index);
                    instantiatedLocals.Add(instantiatedLocal);
                }
                return instantiatedLocals;
            }
        }

        public override MethodIL? MethodIL
        {
            get
            {
                var methodDefinitionIL = GetTypicalMethodDefinition().MethodIL;
                if (methodDefinitionIL == null)
                    return null;
                return new InstantiatedMethodIL(this, methodDefinitionIL);
            }
        }

        public override Instantiation Instantiation => _typicalMethodDef.Instantiation;

        public override bool HasCustomAttribute(string attributeNamespace, string attributeName)
        {
            return _typicalMethodDef.HasCustomAttribute(attributeNamespace, attributeName);
        }

        private TypeDesc Instantiate(TypeDesc type) => type.InstantiateSignature(_instantiatedType.Instantiation, null);

        public override IEnumerable<MethodImplRecord> Overrides => throw new NotImplementedException();
        public override string Name => _typicalMethodDef.Name;
        public override string FullName => ToString();
        public override bool HasReturnType => _typicalMethodDef.HasReturnType;

        public override bool IsVirtual => _typicalMethodDef.IsVirtual;
        public override bool IsAbstract => _typicalMethodDef.IsAbstract;

        public override bool HasThis => _typicalMethodDef.HasThis;
        public override bool IsExplicitThis => _typicalMethodDef.IsExplicitThis;

        public override bool IsIntrinsic => _typicalMethodDef.IsIntrinsic;
        public override MethodDesc CreateUserMethod(string name) => throw new NotImplementedException();

        public override MethodDesc GetTypicalMethodDefinition() => _typicalMethodDef;

        public override MethodDesc GetCanonMethodTarget(CanonicalFormKind kind)
        {
            TypeDesc canonicalizedTypeOfTargetMethod = OwningType.ConvertToCanonForm(kind);
            if (canonicalizedTypeOfTargetMethod == OwningType)
                return this;

            return Context.GetMethodForInstantiatedType(GetTypicalMethodDefinition(), (InstantiatedType)canonicalizedTypeOfTargetMethod);
        }

        public override bool IsCanonicalMethod(CanonicalFormKind policy)
        {
            return OwningType.IsCanonicalSubtype(policy);
        }
    }
}