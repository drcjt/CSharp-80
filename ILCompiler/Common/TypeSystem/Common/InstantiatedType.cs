namespace ILCompiler.Common.TypeSystem.Common
{
    internal class InstantiatedType : DefType
    {
        private readonly MetadataType _typeDef;
        public InstantiatedType(MetadataType typeDef)
        {
            _typeDef = typeDef;
        }

        public override TypeSystemContext Context => _typeDef.Context;

        public override TypeDesc InstantiateSignature(Instantiation? typeInstantiation, Instantiation methodInstantiation)
        {
            throw new NotImplementedException("TODO");
        }
    }
}
