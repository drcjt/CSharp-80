namespace SimpleGenericFields
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

    public static class Tests
    {
        public static int Main()
        {
            int i = 1;
            if (new Gen<int>().Assign(i) != i) return 1;

            string _string = "string";
            if (new Gen<string>().Assign(_string) != _string) return 1;

            var _object = new object();
            if (new Gen<object>().Assign(_object) != _object) return 1;

            return 0;
        }
    }
}