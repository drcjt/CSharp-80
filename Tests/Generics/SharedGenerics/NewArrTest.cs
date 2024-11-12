namespace SharedGenerics
{
    public class NewArrTests
    {
        public class NewArrWrapper<T>
        {
            public object NewArr()
            {
                return new T[10];
            }
        }

        public static bool Run()
        {
            var wrapper = new NewArrWrapper<Foo>();
            object o = wrapper.NewArr();
            return true;
        }
    }
}
