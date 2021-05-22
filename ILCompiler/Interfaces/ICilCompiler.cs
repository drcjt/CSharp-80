namespace ILCompiler.Interfaces
{
    public interface ICilCompiler
    {
        public void Compile(string inputFilePath, string outputFilePath = null);
    }
}
