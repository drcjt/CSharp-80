namespace ILCompiler.Compiler.PreInit
{
    public abstract class Value : ISerializableValue
    {
        public abstract byte[] GetRawData();

        public virtual sbyte AsSByte() => throw new InvalidProgramException();
        public virtual short AsInt16() => throw new InvalidProgramException();
        public virtual int AsInt32() => throw new InvalidProgramException();
    }
}
