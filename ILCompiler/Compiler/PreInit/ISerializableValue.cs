namespace ILCompiler.Compiler.PreInit
{
    /// <summary>
    /// Represents a value resulting from a static constructor that can be serialized
    /// into the generated assembly code directly.
    /// </summary>
    public interface ISerializableValue
    {
        byte[] GetRawData();
    }
}
