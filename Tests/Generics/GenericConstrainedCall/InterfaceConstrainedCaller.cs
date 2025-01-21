namespace GenericConstrainedCall
{
    public class InterfaceConstrainedCaller
    {
        public interface ITester<T>
        {
            void Test();
        }

        public struct MyTester<T> : ITester<T>
        {
            public int Value { get; private set; }

            public void Test()
            {
                Value = 12345;
            }
        }

        public static void DoConstrainedCall<T, U>(ref T t) where T : ITester<U>
        {
            t.Test();
        }
    }
}
