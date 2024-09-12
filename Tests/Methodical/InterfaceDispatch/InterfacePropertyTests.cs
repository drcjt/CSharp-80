namespace InterfaceDispatch
{
    interface IProperties
    {
        int AnInt { get; set; }
    }

    class Properties : IProperties
    {
        private int _anInt;
        public int AnInt
        {
            get
            {
                return _anInt;
            }
            set
            {
                _anInt = value;
            }
        }
    }

    internal static class InterfacePropertyTests
    {
        public static int RunTests()
        {
            IProperties properties = new Properties();
            properties.AnInt = 25;
            if (properties.AnInt != 25)
                return 1;

            return 0;
        }
    }
}