namespace System.Collections.Generic
{
    public interface IEnumerable<out T> : IEnumerable
    {
        new IEnumerator<T> GetEnumerator();
    }

    public static class LowLevelEnumerable
    {
        public static bool Any<T>(this IEnumerable<T> values)
        {
            var enumerator = values.GetEnumerator();
            return enumerator.MoveNext();
        }

        public static int Count<T>(this IEnumerable<T> values)
        {
            int i = 0;
            foreach (var item in values)
                i++;

            return i;
        }

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
