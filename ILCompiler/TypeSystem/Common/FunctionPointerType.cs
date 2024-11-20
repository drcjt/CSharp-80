using ILCompiler.TypeSystem.Canon;

namespace ILCompiler.TypeSystem.Common
{
    public class FunctionPointerType : TypeDesc
    {
        private MethodSignature Signature { get; init; }

        public FunctionPointerType(MethodSignature signature)
        {
            Signature = signature;
        }

        public override TypeSystemContext Context => Signature.ReturnType.Context;

        public override TypeDesc InstantiateSignature(Instantiation? typeInstantiation, Instantiation? methodInstantiation)
        {
            var signatureBuilder = new MethodSignatureBuilder(Signature);
            signatureBuilder.ReturnType = Signature.ReturnType.InstantiateSignature(typeInstantiation, methodInstantiation);
            for (int i = 0; i < Signature.Length; i++)
            {
                signatureBuilder[i] = Signature[i].Type.InstantiateSignature(typeInstantiation, methodInstantiation);
            }

            var instantiatedSignature = signatureBuilder.ToSignature();
            if (instantiatedSignature != Signature)
            {
                return Context.GetFunctionPointerType(instantiatedSignature);
            }

            return this;
        }

        protected override TypeDesc ConvertToCanonFormImpl(CanonicalFormKind kind)
        {
            MethodSignatureBuilder sigBuilder = new MethodSignatureBuilder(Signature);
            sigBuilder.ReturnType = Context.ConvertToCanon(Signature.ReturnType, kind);
            for (int i = 0; i < Signature.Length; i++)
                sigBuilder[i] = Context.ConvertToCanon(Signature[i].Type, kind);

            MethodSignature canonSignature = sigBuilder.ToSignature();
            if (canonSignature != Signature)
                return Context.GetFunctionPointerType(canonSignature);

            return this;
        }

        public override bool IsCanonicalSubtype(CanonicalFormKind policy)
        {
            if (Signature.ReturnType.IsCanonicalSubtype(policy))
                return true;

            for (int i = 0; i < Signature.Length; i++)
            {
                if (Signature[i].Type.IsCanonicalSubtype(policy))
                    return true;
            }

            return false;
        }

        public override bool IsRuntimeDeterminedSubtype
        {
            get
            {
                if (Signature.ReturnType.IsRuntimeDeterminedSubtype)
                    return true;

                for (int i = 0; i < Signature.Length; i++)
                    if (Signature[i].Type.IsRuntimeDeterminedSubtype)
                        return true;

                return false;
            }
        }
    }
}
