using System.Diagnostics.CodeAnalysis;

namespace System
{
    public abstract class ValueType
    {
        public override unsafe bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null || obj.m_pEEType != this.m_pEEType)
                return false;

            // bitwise comparison of the fields
            ref byte thisRawData = ref this.GetRawData();
            ref byte thatRawData = ref obj.GetRawData();

            var result = SpanHelpers.SequenceEqualByte(ref thisRawData, ref thatRawData, (int)this.m_pEEType->ValueTypeSize);

            return result;
        }
    }
}
