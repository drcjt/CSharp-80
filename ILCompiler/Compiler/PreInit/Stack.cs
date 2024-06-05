using ILCompiler.TypeSystem.Common;

namespace ILCompiler.Compiler.PreInit
{
    public class Stack : Stack<StackEntry>
    {
        public void Push(StackValueKind kind, Value value)
        {
            Push(new StackEntry(kind, value));
        }

        public Value PopIntoLocation(TypeDesc locationType)
        {
            var top = Pop();

            switch (top.ValueKind)
            {
                case StackValueKind.Int32:
                    if (!locationType.IsVarType(VarType.Int)
                        && !locationType.IsVarType(VarType.UInt))
                    {
                        int value = top.Value.AsInt32();
                        switch (locationType.VarType)
                        {
                            case VarType.SByte:
                            case VarType.Byte:
                                return ValueTypeValue.FromSByte((sbyte)value);

                            case VarType.Short:
                            case VarType.UShort:
                                return ValueTypeValue.FromInt16((short)value);
                        }
                        throw new InvalidProgramException();
                    }
                    return top.Value;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
