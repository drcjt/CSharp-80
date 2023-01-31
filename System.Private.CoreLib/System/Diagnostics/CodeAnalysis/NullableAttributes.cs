using System;

namespace System.Diagnostics.CodeAnalysis
{
    public sealed class MemberNotNullAttribute : Attribute 
    {
        public MemberNotNullAttribute(string member) => Members = new[] { member };

        public MemberNotNullAttribute(params string[] members) => Members = members;

        public string[] Members { get; }
    }
}
