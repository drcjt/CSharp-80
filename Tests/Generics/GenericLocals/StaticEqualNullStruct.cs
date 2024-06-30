using System;

namespace GenericLocals
{
    internal class StaticEqualNullStruct
    {
        public struct Gen<T>
        {
            public static bool EqualNull(T? t)
            {
                T Field1 = t;
                return ((object)Field1! == null);
            }
        }

        public static int counter = 0;
        public static bool result = true;
        public static void Eval(bool exp)
        {
            counter++;
            if (!exp)
            {
                result = exp;
                Console.Write("StaticEqualNullStruct failed at location: ");
                Console.WriteLine(counter);
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

            return result;
        }
    }
}