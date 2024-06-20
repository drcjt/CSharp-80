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

        public static int RunTests()
        {
            int _int = 1;
            if (new Gen<int>().Assign(_int) != _int) return 1;

            string _string = "string";
            if (new Gen<string>().Assign(_string) != _string) return 2;

            var _object = new object();
            if (new Gen<object>().Assign(_object) != _object) return 3;

            return 0;
        }
    }
}
