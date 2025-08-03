using System;

namespace Xunit
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class MemberDataAttribute : Attribute
    {
        public MemberDataAttribute(string memberName, params object?[] arguments)
        {
        }
    }
}
