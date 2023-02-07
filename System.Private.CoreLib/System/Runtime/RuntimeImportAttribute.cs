namespace System.Runtime
{
    internal class RuntimeImportAttribute : Attribute
    {
        public string EntryPoint { get; }

        public RuntimeImportAttribute(string entry)
        {
            EntryPoint = entry;
        }
    }
}
