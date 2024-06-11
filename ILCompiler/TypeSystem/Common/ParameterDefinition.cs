namespace ILCompiler.TypeSystem.Common
{
    internal class ParameterDefinition
    {
        public readonly TypeDesc Type;
        public readonly string Name;
        public readonly int Index;

        public ParameterDefinition(TypeDesc type, string name, int index)
        {
            Type = type;
            Name = name;
            Index = index;
        }
    }
}