namespace System.Runtime
{
    internal class RuntimeExportAttribute : Attribute
    {
        public string EntryPoint { get; }

        public RuntimeExportAttribute(string entry)
        {
            EntryPoint = entry;
        }
    }
}
