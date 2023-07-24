namespace System.Diagnostics
{
    internal sealed class ConditionalAttribute : Attribute
    {
        public ConditionalAttribute(string conditionString) 
        {
            _conditionString = conditionString;
        }

        private readonly string _conditionString;
        public string ConditionString
        {
            get
            {
                return _conditionString;
            }
        }
    }
}
