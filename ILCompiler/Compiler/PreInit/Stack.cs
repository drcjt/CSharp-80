using dnlib.DotNet;

namespace ILCompiler.Compiler.PreInit
{
    public class Stack : Stack<StackEntry>
    {
        public void Push(StackValueKind kind, Value value)
        {
            Push(new StackEntry(kind, value));
        }

        public Value PopIntoLocation(TypeSig locationType)
        {
            var top = Pop();

            switch (top.ValueKind)
            {
                case StackValueKind.Int32:
                    if (locationType.GetVarType() != VarType.Int 
                        && locationType.GetVarType() != VarType.UInt)
                    {
                        int value = top.Value.AsInt32();
                        switch (locationType.GetVarType())
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
