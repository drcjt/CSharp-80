namespace Z80Assembler
{
    public abstract class Register
    {
        protected Register(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString() => Name;
    }
}
