namespace ILCompiler.Common.TypeSystem.Common
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
    }
}
