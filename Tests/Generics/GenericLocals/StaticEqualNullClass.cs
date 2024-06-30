using System;

namespace GenericLocals
{
    internal class StaticEqualNullClass
    {
        public class Gen<T>
        {
            public static bool EqualNull(T? t)
            {
                T? Field1 = t;
                return ((object?)Field1 == null);
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
                Console.Write("StaticEqualNullClass failed at location: ");
                Console.WriteLine(_counter);
            }
        }

        public static bool RunTests()
        {
            int _int = 1;
            Eval(!Gen<int>.EqualNull(_int));

            string _string = "string";
            Eval(!Gen<string>.EqualNull(_string));
            Eval(Gen<string>.EqualNull(null));

            var _object = new object();
            Eval(!Gen<object>.EqualNull(_object));
            Eval(Gen<object>.EqualNull(null));

            return _result;
        }
    }
}