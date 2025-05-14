namespace System.Reflection
{
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class AssemblyTitleAttribute : Attribute
    {
        public AssemblyTitleAttribute(string title) { Title = title; }
        public string Title { get; }
    }
}