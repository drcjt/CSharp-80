using System.Diagnostics;

namespace ILCompiler.Compiler.ValueNumbering
{
    public class ValueNumberStore
    {
        private readonly Dictionary<int, Dictionary<VNDefFuncApp, ValueNumber>> _vnFuncMaps = [];

        private Dictionary<VNDefFuncApp, ValueNumber> GetFuncMap(int argCount)
        {
            if (!_vnFuncMaps.TryGetValue(argCount, out Dictionary<VNDefFuncApp, ValueNumber>? map))
            {
                map = [];
                _vnFuncMaps[argCount] = map;
            }

            return map;
        }

        public ValueNumber VNForFunc(VarType type, VNFunc func, params ValueNumber[] args)
        {
            Debug.Assert(args.Length <= 4); // Limit to 4 args for now
            Dictionary<VNDefFuncApp, ValueNumber> vnFuncMap = GetFuncMap(args.Length);
            if (!vnFuncMap.TryGetValue(new VNDefFuncApp(func, args), out ValueNumber valueNumber))
            {
                AllocatorType attributes = AllocatorType.Func0 + args.Length;
                ValueNumberAllocator allocator = GetAllocator(type, attributes);
                valueNumber = allocator.Allocate();
                vnFuncMap[new VNDefFuncApp(func, args)] = valueNumber;
            }

            return valueNumber;
        }

        private readonly Dictionary<int, ValueNumber> _intCnsMap = [];

        public ValueNumber VNForIntCon(int cnsVal) => VNForConst(cnsVal, _intCnsMap, VarType.Int);

        private ValueNumber VNForConst<T>(T cnsVal, Dictionary<T, ValueNumber> numMap, VarType type) where T : notnull
        {
            if (!numMap.TryGetValue(cnsVal, out var value))
            {
                ValueNumberAllocator allocator = GetAllocator(type, AllocatorType.Const);
                value = allocator.Allocate();
                numMap[cnsVal] = value;
            }

            return value;
        }

        public ValueNumber VNForExpr(VarType type) => VNForFunc(type, VNFunc.MemOpaque);

        public ValueNumber VNForVoid() => new((int)SpecialRefConsts.SRC_Void);


        public ValueNumber VNForCastOper(VarType type) => VNForIntCon((int)type);

        public ValueNumber VNForCast(ValueNumber sourceVn, VarType targetType, VarType sourceType)
        {
            ValueNumber castTypeVN = VNForCastOper(targetType);
            return VNForFunc(targetType, VNFunc.Cast, sourceVn, castTypeVN);
        }

        private readonly Dictionary<int, ValueNumber> _handleMap = [];

        public ValueNumber VNForHandle(int handleVal)
        {
            Dictionary<int, ValueNumber> handleMap = _handleMap;
            if (!handleMap.TryGetValue(handleVal, out var value))
            {
                ValueNumberAllocator allocator = GetAllocator(VarType.Ref, AllocatorType.Handle);
                value = allocator.Allocate();
                handleMap[handleVal] = value;
            }

            return value;
        }

        private readonly Dictionary<(VarType, AllocatorType), ValueNumberAllocator> _allocators = [];

        private ValueNumberAllocator GetAllocator(VarType type, AllocatorType attributes)
        {
            if (!_allocators.TryGetValue((type, attributes), out var _allocator))
            {
                _allocator = new ValueNumberAllocator();
                _allocators[(type, attributes)] = _allocator;
            }
            return _allocator;
        }
    }
}
