using System;

namespace GenericLocals
{
    internal class StaticAssignmentStruct
    {
        public struct Gen<T>
        {
            public static T Assign(T t)
            {
                T Field1 = t;
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
                Console.Write("StaticAssignmentStruct failed at location: ");
                Console.WriteLine(_counter);
            }
        }

        public static bool RunTests()
        {
            int _int = 1;
            Eval(Gen<int>.Assign(_int) == _int);

            string _string = "string";
            Eval(Gen<string>.Assign(_string) == _string);

            var _object = new object();
            Eval(Gen<object>.Assign(_object) == _object);

            return _result;
        }
    }
}