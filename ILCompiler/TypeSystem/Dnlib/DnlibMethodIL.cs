using dnlib.DotNet.Emit;
using ILCompiler.TypeSystem.Common;
using ILCompiler.TypeSystem.IL;

namespace ILCompiler.TypeSystem.Dnlib
{
    public class DnlibMethodIL : MethodIL
    {
        private readonly List<IL.Instruction> _instructions = new List<IL.Instruction>();

        private readonly ILExceptionRegion[] _exceptionRegions;

        private readonly int _localsCount;
        private readonly bool _isInitLocals;        

        private readonly int _ilCodeSize;

        public DnlibMethodIL(DnlibModule module, CilBody body)
        {
            _ilCodeSize = (int)(body.Instructions[^1].GetOffset() + body.Instructions[^1].GetSize());

            foreach (var instruction in body.Instructions)
            {
                _instructions.Add(new DnlibInstruction(module, instruction));
            }

            _exceptionRegions = new ILExceptionRegion[body.ExceptionHandlers.Count];
            for (int handlerIndex = 0; handlerIndex < body.ExceptionHandlers.Count; handlerIndex++)
            {
                var handler = body.ExceptionHandlers[handlerIndex];
                TypeDesc? catchType = handler.IsCatch ? module.Create(handler.CatchType) : null;
                int tryOffset = (int)(handler.TryStart?.Offset ?? 0);
                int? tryEndOffset = (int?)handler.TryEnd?.Offset;
                int handlerOffset = (int)(handler.HandlerStart?.Offset ?? 0);
                int? handlerEndOffset = (int)(handler.HandlerEnd?.Offset ?? 0);
                int filterOffset = (int)(handler.FilterStart?.Offset ?? 0);

                _exceptionRegions[handlerIndex] = new ILExceptionRegion(GetExceptionRegionKind(handler), tryOffset, tryEndOffset, handlerOffset, handlerEndOffset, filterOffset, catchType);
            }

            _localsCount = body.Variables.Count;
            _isInitLocals = body.InitLocals;

        }

        public override int ILCodeSize => _ilCodeSize;
        public override int LocalsCount => _localsCount;

        public override IList<IL.Instruction> Instructions => _instructions;

        public override ILExceptionRegion[] GetExceptionRegions() => _exceptionRegions;

        public override bool IsInitLocals => _isInitLocals;


        private static ILExceptionRegionKind GetExceptionRegionKind(ExceptionHandler eh)
        {
            if (eh.IsCatch)
                return ILExceptionRegionKind.Catch;
            if (eh.IsFilter)
                return ILExceptionRegionKind.Filter;
            if (eh.IsFault)
                return ILExceptionRegionKind.Fault;
            if (eh.IsFinally)
                return ILExceptionRegionKind.Finally;

            throw new ArgumentException("Invalid exception handler kind");
        }
    }
}
