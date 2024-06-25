namespace GenericTypeParameters
{
    public struct ValX1<T> { }
    public struct ValX2<T, U> { }
    public struct ValX3<T, U, V> { }

    public class RefX1<T> { }
    public class RefX2<T, U> { }
    public class RefX3<T, U, V> { }

    internal class DefaultClass
    {
        public class Gen<T>
        {
            public bool DefaultTest(bool status)
            {
                T? t = default(T);
                return ((object)t == null) == status;
            }
        }

        public static int RunTests()
        {
            if (!new Gen<int>().DefaultTest(false)) return 1;            
            if (!new Gen<string>().DefaultTest(true)) return 2;
            if (!new Gen<object>().DefaultTest(true)) return 3;

            if (!new Gen<RefX1<int>>().DefaultTest(true)) return 4;
            if (!new Gen<RefX1<ValX1<int>>>().DefaultTest(true)) return 5;
            if (!new Gen<RefX2<int, string>>().DefaultTest(true)) return 6;

            if (!new Gen<RefX1<RefX1<int>>>().DefaultTest(true)) return 7;
            if (!new Gen<RefX1<RefX1<RefX1<string>>>>().DefaultTest(true)) return 8;
            if (!new Gen<RefX1<RefX2<int, string>>>().DefaultTest(true)) return 9;
            
            if (!new Gen<RefX2<RefX2<RefX1<int>, RefX3<int, string, RefX1<RefX2<int, string>>>>, RefX2<RefX1<int>, RefX3<int, string, RefX1<RefX2<int, string>>>>>>().DefaultTest(true)) return 10;

            // These fail - Eq on struct not implemented
            /*
            if (!new Gen<ValX1<int>>().DefaultTest(false)) return 11;
            if (!new Gen<ValX1<RefX1<int>>>().DefaultTest(false)) return 12;
            if (!new Gen<ValX2<int, string>>().DefaultTest(false)) return 13;

            if (!new Gen<ValX1<ValX1<int>>>().DefaultTest(false)) return 14;
            if (!new Gen<ValX1<ValX1<ValX1<string>>>>().DefaultTest(false)) return 15;

            if (!new Gen<ValX1<ValX2<int, string>>>().DefaultTest(false)) return 16;
            if (!new Gen<ValX2<ValX2<ValX1<int>, ValX3<int, string, ValX1<ValX2<int, string>>>>, ValX2<ValX1<int>, ValX3<int, string, ValX1<ValX2<int, string>>>>>>().DefaultTest(false)) return 17;
            */

            return 0;
        }
    }
}