using ILCompiler.TypeSystem.Canon;
using ILCompiler.TypeSystem.IL;
using ILCompiler.TypeSystem.Interop;

namespace ILCompiler.TypeSystem.Common
{
    public class InstantiatedMethod : MethodDesc
    {
        private readonly MethodDesc _methodDesc;
        private readonly Instantiation _instantiation;

        public InstantiatedMethod(MethodDesc methodDesc, Instantiation instantiation)
        {
            _methodDesc = methodDesc;
            _instantiation = instantiation;
        }

        public override Instantiation Instantiation { get { return _instantiation; } }

        private TypeDesc Instantiate(TypeDesc type) => type.InstantiateSignature(default(Instantiation), _instantiation);

        public override bool HasCustomAttribute(string attributeNamespace, string attributeName) => _methodDesc.HasCustomAttribute(attributeNamespace, attributeName);

        public override MethodSignature Signature
        {
            get
            {
                var template = _methodDesc.Signature;
                var builder = new MethodSignatureBuilder(template);

                builder.ReturnType = Instantiate(template.ReturnType);
                for (int i = 0; i < template.Length; i++)
                {
                    builder[i] = Instantiate(template[i].Type);
                }

                return builder.ToSignature();
            }
        }

        public override string FullName => ToString();

        public override bool IsDefaultConstructor => _methodDesc.IsDefaultConstructor;

        public override bool HasReturnType => _methodDesc.HasReturnType;

        public override bool IsIntrinsic => _methodDesc.IsIntrinsic;
        public override bool IsPInvoke => _methodDesc.IsPInvoke;
        public override PInvokeMetaData? GetPInvokeMetaData() => _methodDesc.GetPInvokeMetaData();
        public override bool IsInternalCall => _methodDesc.IsInternalCall;
        public override bool IsNoInlining => _methodDesc.IsNoInlining;

        public override bool IsStatic => _methodDesc.IsStatic;

        public override bool IsVirtual => _methodDesc.IsVirtual;
        public override bool IsNewSlot => _methodDesc.IsNewSlot;
        public override bool IsAbstract => _methodDesc.IsAbstract;

        public override bool IsAggressiveInlining => _methodDesc.IsAggressiveInlining;

        public override bool HasThis => _methodDesc.HasThis;

        public override TypeSystemContext Context => _methodDesc.Context;

        public override TypeDesc OwningType => _methodDesc.OwningType;

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

        public override string Name => _methodDesc.Name;

        public override IList<LocalVariableDefinition> Locals
        {
            get
            {
                var instantiatedLocals = new List<LocalVariableDefinition>();
                foreach (var local in _methodDesc.Locals)
                {
                    var instantiatedType = Instantiate(local.Type);
                    var instantiatedLocal = new LocalVariableDefinition(instantiatedType, local.Name, local.Index);
                    instantiatedLocals.Add(instantiatedLocal);
                }
                return instantiatedLocals;
            }
        }

        public override IList<MethodParameter> Parameters
        {
            get
            {
                var instantiatedParameters = new List<MethodParameter>();
                foreach (var parameter in _methodDesc.Parameters)
                {
                    var instantiatedType = Instantiate(parameter.Type);
                    var instantiatedParameter = new MethodParameter(instantiatedType, parameter.Name);
                    instantiatedParameters.Add(instantiatedParameter);
                }

                return instantiatedParameters;
            }
        }

        public override MethodDesc GetMethodDefinition() => _methodDesc;

        // TODO: this shouldn't be used
        public override IEnumerable<MethodImplRecord> Overrides => _methodDesc.Overrides;

        public override MethodDesc GetTypicalMethodDefinition() => _methodDesc.GetTypicalMethodDefinition();

        public override string? GetCustomAttributeValue(string customAttributeName) => _methodDesc.GetCustomAttributeValue(customAttributeName);

        public override MethodDesc CreateUserMethod(string name) => throw new NotImplementedException();

        public override MethodDesc GetCanonMethodTarget(CanonicalFormKind kind)
        {
            InstantiatedMethod canonicalMethodResult = this;
            Instantiation canonInstantiation = Context.ConvertInstantiationToCanonForm(Instantiation, kind, out bool instantiationChanged);
            MethodDesc openMethodOnCanonicalizedType = _methodDesc.GetCanonMethodTarget(kind);

            if (instantiationChanged || (openMethodOnCanonicalizedType != _methodDesc))
            {
                canonicalMethodResult = Context.GetInstantiatedMethod(openMethodOnCanonicalizedType, canonInstantiation);
            }

            return canonicalMethodResult;
        }

        public override bool IsCanonicalMethod(CanonicalFormKind policy)
        {
            if (OwningType.HasInstantiation && OwningType.IsCanonicalSubtype(policy))
            {
                return true;
            }

            for (int i = 0; i < Instantiation.Length; i++)
            {
                var type = Instantiation[i];
                if (type.IsCanonicalSubtype(policy))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
