using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static int Count<T>(this IEnumerable<T> values)
        {
            int count = 0;
            using (IEnumerator<T> enumerator = values.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    count++;
                }
            }

            return count;
        }
    }
}