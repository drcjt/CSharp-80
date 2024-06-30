﻿using System;

namespace GenericFields
{
    internal class StaticEqualNullClass
    {
        public class Gen<T>
        {
            public static T? Field1;

            public bool EqualNull(T? t)
            {
                Field1 = t;
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
                Console.Write("StaticEqualNullClass failed at location: ");
                Console.WriteLine(counter);
            }
        }

        public static bool RunTests()
        {
            int _int = 1;
            Eval(!new Gen<int>().EqualNull(_int));

            string _string = "string";
            Eval(!new Gen<string>().EqualNull(_string));
            Eval(new Gen<string>().EqualNull(null));

            var _object = new object();
            Eval(!new Gen<object>().EqualNull(_object));
            Eval(new Gen<object>().EqualNull(null));

            return result;
        }
    }
}