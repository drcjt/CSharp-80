namespace ILCompiler.Common.TypeSystem.Common
{
    public class LocalVariableDefinition
    {
        public readonly TypeDesc Type;
        public readonly string Name;
        public readonly int Index;

        public LocalVariableDefinition(TypeDesc type, string name, int index)
        {
            Type = type;
            Name = name;
            Index = index;
        }
    }
}
