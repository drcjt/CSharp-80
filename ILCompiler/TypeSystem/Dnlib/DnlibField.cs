using dnlib.DotNet;
using ILCompiler.TypeSystem.Common;

namespace ILCompiler.TypeSystem.Dnlib
{
    public sealed class DnlibField : FieldDesc
    {
        private readonly FieldDef _fieldDef;
        private readonly DnlibModule _module;
        public DnlibField(FieldDef fieldDef, DnlibModule module)
        {
            _fieldDef = fieldDef;
            _module = module;
        }

        public override string Name => _fieldDef.Name;
        public override string FullName => _fieldDef.FullName;

        public override DefType OwningType => (DefType)_module.Create(_fieldDef.DeclaringType);

        public override TypeDesc FieldType
        {
            get
            {
                var fieldTypeSig = _fieldDef.FieldType;
                return _module.Create(fieldTypeSig);
            }
        }

        public override TypeSystemContext Context => _module.Context;

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
