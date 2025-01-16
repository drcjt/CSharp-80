namespace GenericConstrainedCall
{
    class MyInt : IIncrementDecrement
    {
        public int Value { get; private set; }
        public void Increment(int delta) { Value += delta; }
        public void Decrement(int delta) { Value -= delta; }
    }
}