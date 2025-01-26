using Internal.Runtime;
using Internal.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
    public class Array : IList
    {
        // This field should be the first field in Array
        private ushort _numComponents;

        public int Length => Unsafe.As<RawArrayData>(this).Length;

        public unsafe ushort ElementSize => this.GetMethodTable()->ComponentSize;

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

        public static int IndexOf(Array array, object? value)
        {
            return IndexOf(array, value, 0, array.Length);
        }

        public static int IndexOf(Array array, object? value, int startIndex, int count)
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

        public static int IndexOf<T>(T[] array, T value, int startIndex, int count)
        {
            EqualityComparer<T> comparer = EqualityComparerHelpers.GetComparerForReferenceTypesOnly<T>();

            int endIndex = startIndex + count;
            if (comparer != null)
            {
                for (int i = startIndex; i < endIndex; i++)
                {
                    if (comparer.Equals(array[i], value))
                        return i;
                }
            }
            else
            {
                for (int i = startIndex; i < endIndex; i++)
                {
                    if (EqualityComparerHelpers.StructOnlyEquals<T>(array[i], value))
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

        internal unsafe object? InternalGetValue(nint flattenedIndex)
        {
            ref byte element = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetArrayDataReference(this), flattenedIndex * ElementSize);
            EEType* pElementEEType = ElementEEType;

            if (pElementEEType->IsValueType)
            {
                return RuntimeExports.Box(pElementEEType, ref element);
            }
            else
            {
                return Unsafe.As<byte, object?>(ref element);
            }
        }

        internal unsafe void InternalSetValue(object? value, nint flattenedIndex)
        {
            ref byte element = ref Unsafe.AddByteOffset(ref MemoryMarshal.GetArrayDataReference(this), flattenedIndex * ElementSize);
            EEType* pElementEEType = ElementEEType;
            if (pElementEEType->IsValueType)
            {
                RuntimeExports.Unbox(value, ref element, pElementEEType);
            }
            else
            {
                Unsafe.As<byte, object?>(ref element) = value;
            }
        }

        public object? GetValue(int index) => InternalGetValue(index);
        public void SetValue(object? value, int index) => InternalSetValue(value, index);

        internal unsafe EEType* ElementEEType => this.GetMethodTable()->RelatedType;

        public IEnumerator GetEnumerator()
        {
            return new ArrayEnumerator(this);
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Add(object? value)
        {
            throw new NotImplementedException();
        }

        public bool Contains(object? value)
        {
            return IndexOf(value) >= 0;
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object? value) => IndexOf(this, value);

        public void Insert(int index, object? value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object? value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public int Count => Length;

        public object? this[int index]
        {
            get => GetValue(index);
            set => SetValue(value, index);
        }

        private sealed class ArrayEnumerator : IEnumerator
        {
            private readonly Array _array;
            private int _index;
            private readonly int _endIndex; // cache array length, since it's a little slow.

            internal ArrayEnumerator(Array array)
            {
                _array = array;
                _index = -1;
                _endIndex = array.Length;
            }

            public bool MoveNext()
            {
                if (_index < _endIndex)
                {
                    _index++;
                    return (_index < _endIndex);
                }
                return false;
            }

            public void Reset()
            {
                _index = -1;
            }

            public object? Current => _array.InternalGetValue(_index);
        }
    }

    public class Array<T> : Array, IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
