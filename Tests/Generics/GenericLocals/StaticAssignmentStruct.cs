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

        public static int RunTests()
        {
            int _int = 1;
            if (Gen<int>.Assign(_int) != _int) return 1;

            string _string = "string";
            if (Gen<string>.Assign(_string) != _string) return 2;

            var _object = new object();
            if (Gen<object>.Assign(_object) != _object) return 3;

            return 0;
        }
    }
}