using ILCompiler.TypeSystem.Common;

namespace ILCompiler.Compiler.PreInit
{
    public class ValueTypeValue : Value
    {
        private readonly byte[] InstanceBytes;
        public ValueTypeValue(byte[] bytes)
        {
            InstanceBytes = bytes;
        }

        public ValueTypeValue(TypeDesc type)
        {
            InstanceBytes = new byte[type.GetElementSize().AsInt];
        }

        private byte[] AsExactByteCount(int size)
        {
            if (InstanceBytes.Length != size)
            {
                throw new InvalidProgramException();
            }
            return InstanceBytes;
        }

        public override byte[] GetRawData()
        {
            return InstanceBytes;
        }

        public override sbyte AsSByte() => (sbyte)AsExactByteCount(1)[0];
        public override short AsInt16() => BitConverter.ToInt16(AsExactByteCount(2), 0);
        public override int AsInt32() => BitConverter.ToInt32(AsExactByteCount(4), 0);

        public static ValueTypeValue FromSByte(sbyte value) => new ValueTypeValue(new byte[1] { (byte)value });
        public static ValueTypeValue FromInt16(short value) => new ValueTypeValue(BitConverter.GetBytes(value));
        public static ValueTypeValue FromInt32(int value) => new ValueTypeValue(BitConverter.GetBytes(value));
    }
}
