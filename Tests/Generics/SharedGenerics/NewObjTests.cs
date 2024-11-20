namespace SharedGenerics
{
    internal class NewObjTests
    {
        class NewObjWrapper<T>
        {
            public T Newobj(T t1)
            {
                var genericFoo = new GenericFoo<T>(t1);
                return genericFoo.Value;
            }
        }

        class GenericFoo<T>
        {
            public T Value;
            public GenericFoo(T t) 
            { 
                Value = t;
            }
        }

        public static bool Run()
        {
            var newObjWrapper = new NewObjWrapper<int>();
            return newObjWrapper.Newobj(1) == 1;
        }
    }
}
