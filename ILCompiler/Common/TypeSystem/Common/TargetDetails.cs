using dnlib.DotNet;

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

        public LayoutInt GetWellKnownTypeSize(TypeDef type)
        {
            return GetWellKnownTypeSize(type.ToTypeSig());
        }

        public LayoutInt GetWellKnownTypeSize(TypeSig type)
        {
            switch (type.ElementType)
            {
                case ElementType.Void:
                    return new LayoutInt(PointerSize);
                case ElementType.Boolean:
                    return new LayoutInt(1);
                case ElementType.Char:
                    return new LayoutInt(2);
                case ElementType.I1:
                case ElementType.U1:
                    return new LayoutInt(1);
                case ElementType.I2:
                case ElementType.U2:
                    return new LayoutInt(2);
                case ElementType.I4:
                case ElementType.U4:
                    return new LayoutInt(4);
                case ElementType.I:
                case ElementType.U:
                case ElementType.Ptr:
                case ElementType.ByRef:
                case ElementType.Array:
                case ElementType.SZArray:
                    return new LayoutInt(PointerSize);
            }

            // Add new well known types if necessary
            throw new InvalidOperationException($"Cannot get well known type size for type {type.ElementType}");
        }

        public LayoutInt GetWellKnownTypeAlignment(TypeDef type)
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