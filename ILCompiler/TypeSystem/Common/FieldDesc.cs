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
                        // TODO: ComputeStaticFieldLayout
                    }
                    else
                    {
                        OwningType.ComputeInstanceLayout();
                    }
                }
                return _offset;
            }
        }

        internal void InitializeOffset(LayoutInt offset)
        {
            _offset = offset;
        }

        public abstract string Name { get;  }

        public abstract string FullName { get; }

        public abstract DefType OwningType { get; }

        public abstract TypeDesc FieldType { get; }

        public abstract bool IsStatic { get; }

        public abstract bool IsLiteral { get; }
    }
}
