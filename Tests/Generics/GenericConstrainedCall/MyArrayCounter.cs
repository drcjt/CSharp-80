using System;

namespace GenericConstrainedCall
{
    class MyArrayCounter<T>(T[] counters) where T : IIncrementDecrement
    {
        private readonly T[] _counters = counters;

        public void Increment(int index, T item)
        {
            _counters[index] = item;
            _counters[index].Increment(100);
            Console.WriteLine(item.Value);
        }

        public void Decrement(int index)
        {
            _counters[index].Decrement(100);
        }

        public int Value(int index) => _counters[index].Value;
    }
}