using dnlib.DotNet;

namespace ILCompiler.Common.TypeSystem.Common.Dnlib
{
    public sealed class DnlibField : FieldDesc
    {
        private readonly FieldDef _fieldDef;
        private readonly TypeSystemContext _typeSystemContext;
        public DnlibField(FieldDef fieldDef, TypeSystemContext typeSystemContext)
        {
            _fieldDef = fieldDef;
            _typeSystemContext = typeSystemContext;
        }

        public override string Name => _fieldDef.Name;
        public override string FullName => _fieldDef.FullName;

        public override DefType OwningType => (DefType)Context.Create(_fieldDef.DeclaringType);

        public override TypeDesc FieldType
        {
            get
            {
                var fieldTypeSig = _fieldDef.FieldType;
                return Context.Create(fieldTypeSig);
            }
        }

        public override TypeSystemContext Context => _typeSystemContext;

        public override bool IsStatic => _fieldDef.IsStatic;

        public override bool IsLiteral => _fieldDef.IsLiteral;

        public override EffectiveVisibility EffectiveVisibility => _fieldDef.Access switch
        {
            FieldAttributes.Private => EffectiveVisibility.Private,
            FieldAttributes.Public => EffectiveVisibility.Public,
            FieldAttributes.Family => EffectiveVisibility.Family,
            FieldAttributes.Assembly => EffectiveVisibility.Assembly,
            FieldAttributes.FamANDAssem => EffectiveVisibility.FamilyAndAssembly,
            FieldAttributes.FamORAssem => EffectiveVisibility.FamilyOrAssembly,
            _ => throw new NotSupportedException()
        };
    }
}
