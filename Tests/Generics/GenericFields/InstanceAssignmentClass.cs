using System;

namespace GenericFields
{
    internal class InstanceAssignmentClass
    {
        public class Gen<T>
        {
            public T? Field1;

            public T Assign(T t)
            {
                Field1 = t;
                return Field1;
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
                Console.Write("InstanceAssignmentClass failed at location: ");
                Console.WriteLine(counter);
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

            return result;
        }
    }
}