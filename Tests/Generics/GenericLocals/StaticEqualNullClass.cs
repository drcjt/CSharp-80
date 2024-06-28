namespace GenericLocals
{
    internal class StaticEqualNullClass
    {
        public class Gen<T>
        {
            public static bool EqualNull(T? t)
            {
                T Field1 = t;
                return ((object)Field1! == null);
            }
        }

        public static int RunTests()
        {
            int _int = 1;
            if (Gen<int>.EqualNull(_int)) return 1;

            string _string = "string";
            if (Gen<string>.EqualNull(_string)) return 2;
            if (!Gen<string>.EqualNull(null)) return 3;

            var _object = new object();
            if (Gen<object>.EqualNull(_object)) return 4;
            if (!Gen<object>.EqualNull(null)) return 5;

            return 0;
        }
    }
}