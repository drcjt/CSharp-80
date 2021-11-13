namespace System.Runtime.InteropServices
{
    public sealed class DllImportAttribute : Attribute
    {
        public string EntryPoint;
        public CharSet CharSet;
        public DllImportAttribute(string dllName) 
        { 
        }
    }
}
