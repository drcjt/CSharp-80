namespace ILCompiler.TypeSystem.Common
{
    internal class FieldForInstantiatedType : FieldDesc
    {
        private readonly FieldDesc _fieldDef;
        private readonly InstantiatedType _instantiatedType;
        public FieldForInstantiatedType(FieldDesc fieldDef, InstantiatedType instantiatedType)
        {
            _fieldDef = fieldDef;
            _instantiatedType = instantiatedType;
        }

        public override EffectiveVisibility EffectiveVisibility => throw new NotImplementedException();

        public override string Name => _fieldDef.Name;

        public override DefType OwningType => _instantiatedType;

        public override TypeDesc FieldType => _fieldDef.FieldType.InstantiateSignature(_instantiatedType.Instantiation, default(Instantiation));

        public override bool IsStatic => _fieldDef.IsStatic;

        public override bool IsLiteral => _fieldDef.IsLiteral;

        public override bool HasRva => _fieldDef.HasRva;

        public override TypeSystemContext Context => _fieldDef.Context;

        public override FieldDesc GetTypicalFieldDefinition() => _fieldDef;
    }
}
