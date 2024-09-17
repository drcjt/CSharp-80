using System.Runtime.CompilerServices;

namespace System
{
    public class Array
    {
        public extern int Length
        {
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
        }

        public static void Copy(object[] source, object[] destination, int length)
        {
            Copy(source, 0, destination, 0, length);
        }

        public static void Copy(object[] source, int sourceIndex, object[] destination, int destinationIndex, int length)
        {
            if (destinationIndex > sourceIndex)
            {
                for (int i = length - 1; i >= 0; i--)
                {
                    destination[i + destinationIndex] = source[sourceIndex + i];
                }
            }
            else
            {

                for (int i = 0; i < length; i++)
                {
                    destination[i + destinationIndex] = source[sourceIndex + i];
                }
            }
        }

        public static int IndexOf(object[] array, object? value, int startIndex, int count)
        {
            for (int i = startIndex; i < startIndex + count; i++)
            {
                if (array[i].Equals(value))
                    return i;
            }
            return -1;
        }

        public static void Clear(object[] array, int index, int length)
        {
            for (int i = index; i < index + length; i++)
            {
                array[i] = default(object);
            }
        }
    }
}
