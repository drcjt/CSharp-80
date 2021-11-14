using ILCompiler.Compiler;
using Microsoft.Extensions.Logging;

namespace ILCompiler.Interfaces
{
    public interface ICompilation
    {
        public void Compile(string inputFilePath, string outputFilePath);
    }
}
