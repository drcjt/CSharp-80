namespace ILCompiler.IoC
{
    public class Factory<T> where T : class
    {
        private readonly Func<T> _createInstance;
        public Factory(Func<T> createInstance)
        {
            _createInstance = createInstance;
        }

        public T Create()
        {
            return _createInstance();
        }
    }
}
