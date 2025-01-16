namespace GenericConstrainedCall
{
    interface IIncrementDecrement
    {
        void Increment(int delta);
        void Decrement(int delta);
        int Value { get; }
    }
}