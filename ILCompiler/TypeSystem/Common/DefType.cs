using ILCompiler.TypeSystem.Canon;
using ILCompiler.TypeSystem.RuntimeDetermined;

namespace ILCompiler.TypeSystem.Common
{
    public abstract class DefType : TypeDesc
    {
        private bool _computedInstanceLayout;
        private bool _computedStaticLayout;
        private LayoutInt? _instanceFieldSize;
        private LayoutInt? _instanceFieldAlignment;
        private LayoutInt? _instanceByteCountUnaligned;
        private LayoutInt? _instanceByteAlignment;

        private sealed class StaticBlockInfo
        {
            public StaticsBlock NonGcStatics;
            public StaticsBlock GcStatics;
        }

        private StaticBlockInfo? _staticBlockInfo;

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

        public void ComputeStaticFieldLayout()
        {
            var target = new TargetDetails(TargetArchitecture.Z80);
            var fieldLayoutAlgorithm = new MetadataFieldLayoutAlgorithm(target);

            var computedStaticLayout = fieldLayoutAlgorithm.ComputeStaticFieldLayout(this);

            if (computedStaticLayout.NonGcStatics.Size != LayoutInt.Zero ||
                computedStaticLayout.GcStatics.Size != LayoutInt.Zero)
            {
                _staticBlockInfo = new StaticBlockInfo
                {
                    NonGcStatics = computedStaticLayout.NonGcStatics,
                    GcStatics = computedStaticLayout.GcStatics,
                };
            }
            
            if (computedStaticLayout.Offsets != null)
            {
                foreach (var fieldAndOffset in computedStaticLayout.Offsets)
                {
                    fieldAndOffset.Field.InitializeOffset(fieldAndOffset.Offset);
                }
            }

            _computedStaticLayout = true;
        }

        public LayoutInt GCStaticFieldSize
        {
            get
            {
                if (!_computedStaticLayout)
                {
                    ComputeStaticFieldLayout();
                }
                return _staticBlockInfo is null ? LayoutInt.Zero : _staticBlockInfo.GcStatics.Size;
            }
        }

        public LayoutInt NonGCStaticFieldSize
        {
            get
            {
                if (!_computedStaticLayout)
                {
                    ComputeStaticFieldLayout();
                }
                return _staticBlockInfo is null ? LayoutInt.Zero : _staticBlockInfo.NonGcStatics.Size;
            }
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

        protected override TypeDesc ConvertToCanonFormImpl(CanonicalFormKind kind) => this;

        public DefType ConvertToSharedRuntimeDeterminedForm()
        {
            if (Instantiation != null && Instantiation.Length > 0)
            {
                MetadataType typeDefinition = (MetadataType)GetTypeDefinition();

                bool changed;
                Instantiation sharedInstantiation = RuntimeDeterminedTypeUtilities.ConvertInstantiationToSharedRuntimeForm(
                    Instantiation, typeDefinition.Instantiation!, out changed);
                if (changed)
                {
                    return Context.GetInstantiatedType(typeDefinition, sharedInstantiation);
                }
            }

            return this;
        }

        public override bool IsRuntimeDeterminedSubtype
        {
            get
            {
                if (IsGenericDefinition)
                    return false;

                if (Instantiation != null)
                {
                    for (int i = 0; i < Instantiation.Length; i++)
                    {
                        if (Instantiation[i].IsRuntimeDeterminedSubtype)
                            return true;
                    }
                }

                return false;
            }
        }

        public virtual DefType? ContainingType => null;

        public bool ContainsGcPointers
        {
            get
            {
                if (!IsValueType && HasBaseType && BaseType!.ContainsGcPointers)
                {
                    return true;
                }

                if (MetadataFieldLayoutAlgorithm.ComputeContainsGcPointers(this))
                {
                    return true;
                }

                return false;
            }
        }
    }
}
