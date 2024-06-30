using System;

namespace GenericFields
{
    internal class StaticAssignmentClass
    {
        public class Gen<T>
        {
            static private T? Field1;

            public T Assign(T t)
            {
                Field1 = t;
                return Field1;
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
                Console.Write("StaticAssignmentClass failed at location: ");
                Console.WriteLine(_counter);
            }
        }

        public static bool RunTests()
        {
            int _int = 1;
            Eval(new Gen<int>().Assign(_int) == _int);

            string _string = "string";
            Eval(new Gen<string>().Assign(_string) == _string);

            var _object = new object();
            Eval(new Gen<object>().Assign(_object) == _object);

            return _result;
        }
    }
}
