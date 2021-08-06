using System.IO;

namespace Z80Assembler
{
    public class StreamCodeWriter : ICodeWriter
    {
        public StreamCodeWriter(Stream stream) => Stream = stream;

        public readonly Stream Stream;

        public void WriteByte(byte value) => Stream.WriteByte(value);
    }
}
