using System;

namespace GenericTypeParameters
{
    internal class DefaultStruct
    {
        public struct Gen<T>
        {
            public bool DefaultTest(bool status)
            {
                T? t = default(T);
                return (((object?)t == null) == status);
            }
        }

        private static int _counter = 0;
        private static bool _result = true;
        public static void Eval(bool exp)
        {
            _counter++;
            if (!exp)
            {
                _result = exp;
                Console.Write("DefaultStruct failed at location: ");
                Console.WriteLine(_counter);
            }
        }

        public static bool RunTests()
        {
            Eval(new Gen<int>().DefaultTest(false));
            Eval(new Gen<string>().DefaultTest(true));
            Eval(new Gen<object>().DefaultTest(true));

            Eval(new Gen<RefX1<int>>().DefaultTest(true));
            Eval(new Gen<RefX1<ValX1<int>>>().DefaultTest(true));
            Eval(new Gen<RefX2<int, string>>().DefaultTest(true));

            Eval(new Gen<RefX1<RefX1<int>>>().DefaultTest(true));
            Eval(new Gen<RefX1<RefX1<RefX1<string>>>>().DefaultTest(true));
            Eval(new Gen<RefX1<RefX2<int, string>>>().DefaultTest(true));

            Eval(new Gen<RefX2<RefX2<RefX1<int>, RefX3<int, string, RefX1<RefX2<int, string>>>>, RefX2<RefX1<int>, RefX3<int, string, RefX1<RefX2<int, string>>>>>>().DefaultTest(true));

            Eval(new Gen<ValX1<int>>().DefaultTest(false));
            Eval(new Gen<ValX1<RefX1<int>>>().DefaultTest(false));
            Eval(new Gen<ValX2<int, string>>().DefaultTest(false));

            Eval(new Gen<ValX1<ValX1<int>>>().DefaultTest(false));
            Eval(new Gen<ValX1<ValX1<ValX1<string>>>>().DefaultTest(false));

            Eval(new Gen<ValX1<ValX2<int, string>>>().DefaultTest(false));
            Eval(new Gen<ValX2<ValX2<ValX1<int>, ValX3<int, string, ValX1<ValX2<int, string>>>>, ValX2<ValX1<int>, ValX3<int, string, ValX1<ValX2<int, string>>>>>>().DefaultTest(false));

            return _result;
        }
    }
}