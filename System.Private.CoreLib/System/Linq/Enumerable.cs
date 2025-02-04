namespace System.Collections.Generic
{
    public static partial class Enumerable
    {
        public static List<TSource> ToList<TSource>(this IEnumerable<TSource> values)
        {
            return new List<TSource>(values);
        }

        public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
        {
            ArrayBuilder<TSource> arrayBuilder = default;
            foreach (TSource value in source)
            {
                arrayBuilder.Add(value);
            }
            return arrayBuilder.ToArray();
        }
    }
}
