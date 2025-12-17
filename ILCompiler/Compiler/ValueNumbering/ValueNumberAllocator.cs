namespace ILCompiler.Compiler.ValueNumbering
{
    enum AllocatorType
    {
        Const,
        PhiDef,
        Func0,
        Func1,
        Func2,
        Func3,
        Func4,
        Handle,
    }

    enum SpecialRefConsts
    {
        SRC_Null,
        SRC_Void,
    }

    public class ValueNumberAllocator
    {
        private int _nextValueNumber = (int)(Enum.GetValues<SpecialRefConsts>().Max()) + 1;
        public ValueNumber Allocate()
        {
            return new ValueNumber(_nextValueNumber++);
        }
    }
}
