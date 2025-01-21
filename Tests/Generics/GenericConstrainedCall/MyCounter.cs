namespace GenericConstrainedCall
{
    class MyCounter<T>(T counter) where T : IIncrementDecrement
    {
        private T _counter = counter;

        public void Increment()
        {
            _counter.Increment(100);
        }

        public virtual void Increment(T counter)
        {
            counter.Increment(100);
            _counter = counter;
        }

        public void Decrement()
        {
            _counter.Decrement(100);
        }

        public virtual void Decrement(T counter)
        {
            counter.Decrement(100);
            _counter = counter;
        }

        public int Value => _counter.Value;
    }
}