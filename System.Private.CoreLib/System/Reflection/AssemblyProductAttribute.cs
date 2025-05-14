﻿namespace System.Reflection
{
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class AssemblyProductAttribute : Attribute
    {
        public AssemblyProductAttribute(string product) { Product = product; }
        public string Product { get; }
    }
}