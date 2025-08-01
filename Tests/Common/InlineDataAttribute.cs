using System;

namespace Xunit
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class InlineDataAttribute : Attribute
    {
        public InlineDataAttribute(params object?[] dataValues)
        {
        }
    }
}
