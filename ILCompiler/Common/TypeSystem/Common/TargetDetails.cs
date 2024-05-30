namespace ILCompiler.Common.TypeSystem.Common
{
    public enum TargetArchitecture
    {
        Unknown = 0,
        Z80,
    }

    public class TargetDetails
    {
        public TargetDetails(TargetArchitecture architecture)
        {
            Architecture = architecture;
        }

        public TargetArchitecture Architecture
        {
            get;
        }

        public int MaximumAlignment
        {
            get
            {
                if (Architecture == TargetArchitecture.Z80)
                {
                    // TODO: May need to remove this when implementing longs & doubles
                    return 4;
                }

                // TODO: Why is this an appropriate default??
                return 32;
            }
        }

        public int PointerSize
        {
            get
            {
                switch (Architecture)
                {
                    case TargetArchitecture.Z80:
                        return 2;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public LayoutInt LayoutPointerSize => new LayoutInt(PointerSize);

        public int DefaultPackingSize
        {
            get
            {
                // Use default packing size of 32 for all platforms
                return 32;
            }
        }

        public int MaximumLog2PrimitiveSize => 3;
        public int MaximumPrimitiveSize => 1 << MaximumLog2PrimitiveSize;

        public LayoutInt GetWellKnownTypeSize(DefType type)
        {
            switch (type.Category)
            {
                case TypeFlags.Void:
                    return new LayoutInt(PointerSize);
                case TypeFlags.Boolean:
                    return new LayoutInt(1);
                case TypeFlags.Char:
                    return new LayoutInt(2);
                case TypeFlags.Byte:
                case TypeFlags.SByte:
                    return new LayoutInt(1);
                case TypeFlags.Int16:
                case TypeFlags.UInt16:
                    return new LayoutInt(2);
                case TypeFlags.Int32:
                case TypeFlags.UInt32:
                    return new LayoutInt(4);
                //case ElementType.Pinned:
                    //return GetWellKnownTypeSize(type.Next);
                case TypeFlags.IntPtr:
                case TypeFlags.UIntPtr:
//                case TypeFlags.Ptr:
                case TypeFlags.ByRef:
                case TypeFlags.Array:
                case TypeFlags.SzArray:
                    return new LayoutInt(PointerSize);
            }

            // Add new well known types if necessary
            throw new InvalidOperationException($"Cannot get well known type size for type {type.Category}");
        }

        public LayoutInt GetWellKnownTypeAlignment(DefType type)
        {
            // The size is the alignment
            return GetWellKnownTypeSize(type);
        }

        /// <summary>
        /// Given an alignment of the fields of a type, determine the alignment that is necessary for allocating the object on the heap
        /// </summary>
        /// <param name="fieldAlignment"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public LayoutInt GetObjectAlignment(LayoutInt fieldAlignment)
        {
            switch (Architecture)
            {
                case TargetArchitecture.Z80:
                    return new LayoutInt(2);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}