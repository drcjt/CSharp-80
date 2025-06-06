﻿namespace System.Reflection
{
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
    public sealed class AssemblyCompanyAttribute : Attribute
    {
        public AssemblyCompanyAttribute(string company) { Company = company; }
        public string Company { get; }
    }
}