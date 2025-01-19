namespace System.Runtime.CompilerServices
{
    [InterpolatedStringHandler]
    public class DefaultInterpolatedStringHandler
    {
        private string result = "";
        public DefaultInterpolatedStringHandler(int literalLength, int formattedCount)
        {
        }

        public void AppendLiteral(string value)
        {
            result += value;
        }

        public void AppendFormatted<T>(T value)
        {
            result += value?.ToString();
        }

        public string ToStringAndClear()
        {
            var builtString = result;
            result = "";
            return builtString;
        }
    }
}
