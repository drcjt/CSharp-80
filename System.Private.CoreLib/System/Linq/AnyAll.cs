using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static bool Any<T>(this IEnumerable<T> values)
        {
            var enumerator = values.GetEnumerator();
            return enumerator.MoveNext();
        }
    }
}
