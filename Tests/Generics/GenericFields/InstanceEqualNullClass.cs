namespace GenericFields
{
    internal class InstanceEqualNullClass
    {
        public class Gen<T>
        {
            public T Field1;

            public bool EqualNull(T t)
            {
                Field1 = t;
                return ((object)Field1 == null);
            }
        }

        public static int RunTests()
        {
            int _int = 1;
            if (new Gen<int>().EqualNull(_int)) return 1;

            string _string = "string";
            if (new Gen<string>().EqualNull(_string)) return 2;
            if (!new Gen<string>().EqualNull(null)) return 2;

            var _object = new object();
            if (new Gen<object>().EqualNull(_object)) return 3;
            if (!new Gen<object>().EqualNull(null)) return 3;

            return 0;
        }
    }
}