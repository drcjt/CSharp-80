namespace ILCompiler.TypeSystem.Common
{
    public class InstantiatedType : DefType
    {
        private readonly MetadataType _typeDef;
        private readonly Instantiation _instantiation;

        public InstantiatedType(MetadataType typeDef, Instantiation instantiation)
        {
            _typeDef = typeDef;
            _instantiation = instantiation;
        }

        public override TypeSystemContext Context => _typeDef.Context;

        public override TypeDesc InstantiateSignature(Instantiation? typeInstantiation, Instantiation? methodInstantiation)
        {
            var clone = new TypeDesc[_instantiation.Length];
            for (int i = 0; i < _instantiation.Length; i++)
            {
                var uninstantiated = _instantiation[i];
                var instantiated = uninstantiated.InstantiateSignature(typeInstantiation, methodInstantiation);   
                clone[i] = instantiated;
            }

            return Context.GetInstantiatedType(_typeDef, new Instantiation(clone));
        }

        public override Instantiation? Instantiation => _instantiation;
    }
}
