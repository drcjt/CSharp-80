namespace GenericFields
{
    internal class InstanceEqualNullStruct
    {
        public struct Gen<T>
        {
            public T? Field1;

            public bool EqualNull(T? t)
            {
                Field1 = t;
                return ((object)Field1! == null);
            }
        }

        public static int RunTests()
        {
            int _int = 1;
            if (new Gen<int>().EqualNull(_int)) return 1;

            string _string = "string";
            if (new Gen<string>().EqualNull(_string)) return 2;
            if (!new Gen<string>().EqualNull(null)) return 3;

            var _object = new object();
            if (new Gen<object>().EqualNull(_object)) return 4;
            if (!new Gen<object>().EqualNull(null)) return 5;

            return 0;
        }
    }
}