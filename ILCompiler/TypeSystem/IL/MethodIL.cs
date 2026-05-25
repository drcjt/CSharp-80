using ILCompiler.TypeSystem.Common;

namespace ILCompiler.TypeSystem.IL
{
    public class MethodIL
    {
        public virtual IList<Instruction> Instructions { get; set; } = new List<Instruction>();

        public virtual ILExceptionRegion[] GetExceptionRegions() => [];
        public virtual bool IsInitLocals { get; }

        public virtual int LocalsCount { get; set; }

        public virtual int ILCodeSize => 0;

        public List<LocalVariableDefinition> Locals { get; set; } = new List<LocalVariableDefinition>();

        public virtual MethodIL GetMethodILDefinition()
        {
            return this;
        }
    }

    public enum ILExceptionRegionKind
    {
        Catch = 0,
        Fault = 1,
        Filter = 2,
    }

    public class ILExceptionRegion
    {
        public ILExceptionRegionKind Kind { get; }
        public uint TryOffset { get; }
        public uint? TryEndOffset { get; }
        public uint HandlerOffset { get; }
        public uint? HandlerEndOffset { get; }

        public uint? FilterOffset { get; }
        public TypeDesc? CatchType { get; }

        public ILExceptionRegion(ILExceptionRegionKind kind, uint tryOffset, uint? tryEndOffset, uint handlerOffset, uint? handlerEndOffset, uint? filterOffset, TypeDesc? catchType)
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
