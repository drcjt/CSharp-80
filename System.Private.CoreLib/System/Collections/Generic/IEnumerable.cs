namespace System.Collections.Generic
{
    public interface IEnumerable<out T> : IEnumerable
    {
        new IEnumerator<T> GetEnumerator();
    }

    public static class LowLevelEnumerable
    {
        public static T[] ToArray<T>(this IEnumerable<T> values)
        {
            ArrayBuilder<T> arrayBuilder = default;
            foreach (T value in values)
            {
                arrayBuilder.Add(value);
            }
            return arrayBuilder.ToArray();
        }
    }
}
