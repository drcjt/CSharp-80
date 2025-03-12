using ILCompiler.TypeSystem.Canon;
using ILCompiler.TypeSystem.Common;

namespace ILCompiler.TypeSystem.RuntimeDetermined
{
    public class RuntimeDeterminedType : DefType
    {
        public DefType CanonicalType { get; init; }
        public GenericParameterDesc RuntimeDeterminedDetailsType { get; init; }

        public RuntimeDeterminedType(DefType rawCanonType, GenericParameterDesc runtimeDeterminedDetailsType)
        {
            CanonicalType = rawCanonType;
            RuntimeDeterminedDetailsType = runtimeDeterminedDetailsType;
        }

        public override TypeSystemContext Context => CanonicalType.Context;

        public override DefType? BaseType => CanonicalType.BaseType;

        public override Instantiation? Instantiation => CanonicalType.Instantiation;
        public override string Name => CanonicalType.Name;
        public override string Namespace => string.Concat(RuntimeDeterminedDetailsType.Name, "_", CanonicalType.Namespace);
        public override IEnumerable<MethodDesc> GetMethods()
        {
            foreach (var method in CanonicalType.GetMethods())
            {
                yield return Context.GetMethodForRuntimeDeterminedType(method.GetTypicalMethodDefinition(), this);
            }
        }

        public override IEnumerable<MethodDesc> GetVirtualMethods()
        {
            foreach (var method in CanonicalType.GetVirtualMethods())
            {
                yield return Context.GetMethodForRuntimeDeterminedType(method.GetTypicalMethodDefinition(), this);
            }
        }

        public override TypeDesc GetTypeDefinition()
        {
            if (CanonicalType.HasInstantiation)
            {
                return Context.GetRuntimeDeterminedType((DefType)CanonicalType.GetTypeDefinition(), RuntimeDeterminedDetailsType);
            }
            return this;
        }

        public override bool IsCanonicalSubtype(CanonicalFormKind policy) => false;

        public override bool IsRuntimeDeterminedSubtype => true;

        public override MethodDesc? GetMethodWithEquivalentSignature(string name, MethodSignature? signature, Instantiation? instantiation)
        {
            MethodDesc? method = CanonicalType.GetMethodWithEquivalentSignature(name, signature, instantiation);
            if (method == null)
                return null;
            return Context.GetMethodForRuntimeDeterminedType(method.GetTypicalMethodDefinition(), this);
        }
    }
}
