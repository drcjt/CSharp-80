using System;

namespace Xunit
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TheoryAttribute : FactAttribute
    {
    }
}
