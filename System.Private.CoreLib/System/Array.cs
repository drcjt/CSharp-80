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

        public static void Copy(object?[] source, object?[] destination, int length)
        {
            Copy(source, 0, destination, 0, length);
        }

        public static void Copy<T>(T[] source, T[] destination, int length)
        {
            Copy<T>(source, 0, destination, 0, length);
        }

        public static void Copy<T>(T[] source, int sourceIndex, T[] destination, int destinationIndex, int length)
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
                // TODO: Buffer.Memmove(ref destination[destinationIndex], ref source[sourceIndex + 1], (nuint)length);

                for (int i = 0; i < length; i++)
                {
                    destination[i + destinationIndex] = source[sourceIndex + i];
                }
            }
        }


        public static void Copy(object?[] source, int sourceIndex, object?[] destination, int destinationIndex, int length)
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

        public static int IndexOf(object?[] array, object? value, int startIndex, int count)
        {
            int endIndex = startIndex + count;
            if (value is null)
            {
                for (int i = startIndex; i < endIndex; i++)
                {
                    if (array[i] is null)
                    {
                        return i;
                    }
                }
            }
            else
            {
                for (int i = startIndex; i < endIndex; i++)
                {
                    var obj = array[i];
                    if (obj is not null && obj.Equals(value))
                        return i;
                }
            }
            return -1;
        }

        public static int IndexOf<T>(T[] array, object? value, int startIndex, int count)
        {
            
            int endIndex = startIndex + count;
            if (value is null)
            {
                for (int i = startIndex; i < endIndex; i++)
                {
                    if (array[i] is null)
                    {
                        return i;
                    }
                }
            }
            else
            {
                for (int i = startIndex; i < endIndex; i++)
                {
                    object? obj = array[i];
                    if (obj is not null && obj.Equals(value))
                        return i;
                }
            }
            return -1;
        }

        public static void Clear(object?[] array, int index, int length)
        {
            for (int i = index; i < index + length; i++)
            {
                array[i] = default;
            }
        }

        public static void Clear<T>(T?[] array, int index, int length)
        {
            for (int i = index; i < index + length; i++)
            {
                array[i] = default;
            }
        }
    }
}
