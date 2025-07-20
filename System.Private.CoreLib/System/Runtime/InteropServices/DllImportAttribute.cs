namespace System.Runtime.InteropServices
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class DllImportAttribute : Attribute
    {
        public string? EntryPoint { get; set; }
        public CharSet CharSet { get; set; }
        public DllImportAttribute(string dllName) 
        { 
        }
    }
}
