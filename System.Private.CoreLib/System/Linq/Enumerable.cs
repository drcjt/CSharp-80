using System.Collections.Generic;

namespace System.Linq
{
    public static class Enumerable
    {
        public static List<TSource> ToList<TSource>(this IEnumerable<TSource> values)
        {
            return new List<TSource>(values);
        }
    }
}
