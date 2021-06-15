namespace ILCompiler.z80
{
    public class Condition
    {
        private readonly string _name;
        public Condition(string name)
        {
            _name = name;
        }

        public static readonly Condition Zero = new("Z");
        public static readonly Condition NonZero = new("NZ");
        public static readonly Condition C = new("C");

        public override string ToString()
        {
            return _name;
        }
    }
}
