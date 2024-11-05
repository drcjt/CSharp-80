namespace SharedGenerics
{
    internal class NewObjTests
    {
        static GenericFoo<T> Newobj<T>(T t1)
        {
            return new GenericFoo<T>(t1);
        }

        public static int Run()
        {
            var g1 = Newobj(1);
            return 0;
        }
    }
}
