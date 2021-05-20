namespace ILCompiler.z80
{
    public abstract class Register
    {
        public Register(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString() => Name;
    }
}
