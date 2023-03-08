namespace System.Diagnostics.CodeAnalysis
{
    public sealed class DynamicDependencyAttribute : Attribute
    {
        public DynamicDependencyAttribute(string memberSignature)
        {
            MemberSignature = memberSignature;
        }

        public string? MemberSignature { get; }
    }
}
