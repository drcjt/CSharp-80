using System.Runtime.Serialization;

namespace ILCompiler.Compiler
{
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
    }
}
