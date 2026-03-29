using System;

namespace Xunit
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ActiveIssueAttribute : Attribute
    {
        public ActiveIssueAttribute(string issue) => Issue = issue;
        public string Issue { get; }
    }
}
