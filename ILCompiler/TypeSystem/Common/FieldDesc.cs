namespace ILCompiler.TypeSystem.Common
{
    public enum EffectiveVisibility
    {
        Private,
        Public,
        Family,
        Assembly,
        FamilyAndAssembly,
        FamilyOrAssembly,
    }

    public abstract class FieldDesc : TypeSystemEntity
    {
        public static readonly FieldDesc[] EmptyFields = [];

        public abstract EffectiveVisibility EffectiveVisibility { get; }

        private LayoutInt _offset = FieldAndOffset.InvalidOffset;

        public LayoutInt Offset
        {
            get
            {
                if (_offset == FieldAndOffset.InvalidOffset)
                {
                    if (IsStatic)
                    {
                        OwningType.ComputeStaticFieldLayout();
                    }
                    else
                    {
                        OwningType.ComputeInstanceLayout();
                    }
                }
                return _offset;
            }
        }

        public bool HasGcStaticBase => Context.ComputeHasGcStaticBase(this);

        internal void InitializeOffset(LayoutInt offset)
        {
            _offset = offset;
        }

        public abstract string Name { get;  }

        public abstract DefType OwningType { get; }

        public abstract TypeDesc FieldType { get; }

        public abstract bool IsStatic { get; }

        public abstract bool IsLiteral { get; }

        public abstract bool HasRva { get; }

        public virtual FieldDesc InstantiateSignature(Instantiation? typeInstantiation, Instantiation? methodInstantiation)
        {
            var field = this;
            var instantiatedOwningType = OwningType.InstantiateSignature(typeInstantiation, methodInstantiation);
            if (instantiatedOwningType != OwningType)
            {
                field = instantiatedOwningType.Context.GetFieldForInstantiatedType(field.GetTypicalFieldDefinition(), (InstantiatedType)instantiatedOwningType);
            }

            return field;
        }

        public virtual FieldDesc GetTypicalFieldDefinition()
        {
            return this;
        }

        public override string ToString()
        {
            return $"{OwningType}.{Name}";
        }
    }
}
