namespace System
{
    public readonly struct Boolean
        : IComparable,
          IEquatable<bool>,
          IComparable<bool>
    {
        private readonly bool m_value;

        internal const int True = 1;
        internal const int False = 0;

        internal const string TrueLiteral = "True";
        internal const string FalseLiteral = "False";

        public static readonly string TrueString = TrueLiteral;
        public static readonly string FalseString = FalseLiteral;

        public override int GetHashCode() => m_value ? True : False;

        public override string ToString()
        {
            if (false == m_value)
            {
                return FalseLiteral;
            }
            return TrueLiteral;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not bool)
                return false;
            return m_value == ((bool)obj).m_value;
        }

        public bool Equals(bool obj)
        {
            return m_value == obj;
        }

        public int CompareTo(object? obj)
        {
            if (obj is null)
            {
                return 1;
            }

            if (obj is bool b)
            {
                if (m_value == b)
                {
                    return 0;
                }
                else if (!m_value)
                {
                    return -1;
                }

                return 1;
            }

            throw new ArgumentException();
        }

        public int CompareTo(bool value)
        {
            if (m_value == value)
            {
                return 0;
            }
            else if (!m_value)
            {
                return -1;
            }
            return 1;
        }
    }
}
