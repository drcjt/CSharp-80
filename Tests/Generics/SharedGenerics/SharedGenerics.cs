namespace SharedGenerics
{
    public class Foo
    {
        public int i, j, k;
    }

    class GenericFoo<T>
    {
        public GenericFoo(T i1)
        {
        }
    }

    internal static class TestRunner
    {

        public static int Main()
        {
            var result = NewArrTests.Run();
            result &= NewObjTests.Run();

            return result ? 0 : 1;
        }
    }
}
