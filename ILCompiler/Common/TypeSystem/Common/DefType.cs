namespace ILCompiler.Common.TypeSystem.Common
{
    public abstract class DefType : TypeDesc
    {
        private bool _computedInstanceLayout;
        private LayoutInt? _instanceFieldSize;
        private LayoutInt? _instanceFieldAlignment;
        private LayoutInt? _instanceByteCountUnaligned;
        private LayoutInt? _instanceByteAlignment;

        public void ComputeInstanceLayout()
        {
            var target = new TargetDetails(TargetArchitecture.Z80);
            var fieldLayoutAlgorithm = new MetadataFieldLayoutAlgorithm(target);

            var computedLayout = fieldLayoutAlgorithm.ComputeInstanceLayout(this);
            _instanceFieldSize = computedLayout.FieldSize;
            _instanceFieldAlignment = computedLayout.FieldAlignment;
            _instanceByteCountUnaligned = computedLayout.ByteCountUnaligned;
            _instanceByteAlignment = computedLayout.ByteCountAlignment;

            if (computedLayout.Offsets != null)
            {
                foreach (var fieldAndOffset in computedLayout.Offsets)
                {
                    fieldAndOffset.Field.InitializeOffset(fieldAndOffset.Offset);
                }
            }

            _computedInstanceLayout = true;
        }

        public LayoutInt InstanceFieldSize
        {
            get
            {
                if (!_computedInstanceLayout)
                {
                    ComputeInstanceLayout();
                }
                return _instanceFieldSize!;
            }
        }

        public LayoutInt InstanceFieldAlignment
        {
            get
            {
                if (!_computedInstanceLayout)
                {
                    ComputeInstanceLayout();
                }
                return _instanceFieldAlignment!;
            }
        }

        public LayoutInt InstanceByteCountUnaligned
        {
            get
            {
                if (!_computedInstanceLayout)
                {
                    ComputeInstanceLayout();
                }
                return _instanceByteCountUnaligned!;
            }
        }

        public LayoutInt InstanceByteAlignment
        {
            get
            {
                if (!_computedInstanceLayout)
                {
                    ComputeInstanceLayout();
                }
                return _instanceByteAlignment!;
            }
        }

        public LayoutInt InstanceByteCount => LayoutInt.AlignUp(InstanceByteCountUnaligned, InstanceByteAlignment, Context.Target);
    }
}