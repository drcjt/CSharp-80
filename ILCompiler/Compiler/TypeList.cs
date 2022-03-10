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
                    return 4;
                case StackValueKind.NativeInt:
                    return 4;
                case StackValueKind.ByRef:
                    return 4;
                default:
                    throw new NotImplementedException($"Kind {kind} not yet supported");
            }
        }
    }
}
