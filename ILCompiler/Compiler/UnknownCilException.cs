using System.Runtime.Serialization;

namespace ILCompiler.Compiler
{
    [Serializable]
    public class UnknownCilException : Exception
    {
        public UnknownCilException()
        {
        }

        public UnknownCilException(string message) : base(message)
        {
        }

        public UnknownCilException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UnknownCilException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
