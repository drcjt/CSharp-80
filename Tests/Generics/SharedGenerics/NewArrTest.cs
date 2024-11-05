namespace SharedGenerics
{
    public class NewArrTests
    {
        static object Newarr<T>()
        {
            object o = new T[10];
            return o;
        }

        public static int Run()
        {
            object o = Newarr<Foo>();
            return 0;
        }
    }
}
