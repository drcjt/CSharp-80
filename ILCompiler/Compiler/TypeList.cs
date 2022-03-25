using ILCompiler.Common.TypeSystem.IL;

namespace ILCompiler.Compiler
{
    public static class TypeList
    {
        public static int GetExactSize(StackValueKind kind)
        {
            switch (kind)
            {
                case StackValueKind.Int32:
                    return 4;
                case StackValueKind.Int64:
                    return 8;
                case StackValueKind.ObjRef:
                    return 2;
                case StackValueKind.NativeInt:
                    return 2;
                case StackValueKind.ByRef:
                    return 2;
                default:
                    throw new NotImplementedException($"Kind {kind} not yet supported");
            }
        }
    }
}
