﻿namespace System.Runtime.InteropServices
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class DllImportAttribute : Attribute
    {
        public string? EntryPoint;
        public CharSet CharSet;
        public DllImportAttribute(string dllName) 
        { 
        }
    }
}
