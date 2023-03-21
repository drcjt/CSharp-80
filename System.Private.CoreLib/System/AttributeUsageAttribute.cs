namespace System
{
    public sealed class AttributeUsageAttribute : Attribute
    {
        private readonly AttributeTargets _attributeTarget;
        private bool _allowMultiple;
        private bool _inherited;

        public AttributeUsageAttribute(AttributeTargets validOn) 
        {
            _attributeTarget = validOn;
            _inherited = true;
        }

        public AttributeTargets ValidOn => _attributeTarget;

        public bool AllowMultiple
        {
            get => _allowMultiple;
            set => _allowMultiple = value;
        }

        public bool Inherited
        {
            get => _inherited; 
            set => _inherited = value;
        }
    }
}
