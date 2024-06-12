using ILCompiler.TypeSystem.Common;

namespace ILCompiler.TypeSystem.IL
{
    public abstract class MethodIL
    {
        public abstract IList<Instruction> Instructions { get; }

        public abstract ILExceptionRegion[] GetExceptionRegions();
        public abstract bool IsInitLocals { get; }

        public abstract int LocalsCount { get; }
    }

    public enum ILExceptionRegionKind
    {
        Catch = 0,
        Filter = 1,
        Finally = 2,
        Fault = 4,
    }

    public class ILExceptionRegion
    {
        public ILExceptionRegionKind Kind { get; }
        public int TryOffset { get; }
        public int? TryEndOffset { get; }
        public int HandlerOffset { get; }
        public int? HandlerEndOffset { get; }

        public int FilterOffset { get; }
        public TypeDesc? CatchType { get; }

        public ILExceptionRegion(ILExceptionRegionKind kind, int tryOffset, int? tryEndOffset, int handlerOffset, int? handlerEndOffset, int filterOffset, TypeDesc? catchType)
        {
            Kind = kind;
            TryOffset = tryOffset;
            TryEndOffset = tryEndOffset;
            HandlerOffset = handlerOffset;
            HandlerEndOffset = handlerEndOffset;
            FilterOffset = filterOffset;
            CatchType = catchType;
        }
    }
}