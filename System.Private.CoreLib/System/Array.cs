using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Internal.Runtime;
using Internal.Runtime.CompilerServices;

namespace System
{
    public abstract class Array : IList
    {
        // This field should be the first field in Array
        private ushort _numComponents;

        // Stop C# from generating a protected constructor
        private protected Array() { }

        public int Length => Unsafe.As<RawArrayData>(this).Length;

        public static int MaxLength => 65535;

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

        private static class EmptyArray<T>
        {
            internal static readonly T[] Value = new T[0];
        }

        public static T[] Empty<T>() => EmptyArray<T>.Value;

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

            public void Reset() => _index = -1;

            public object? Current => _array.InternalGetValue(_index);
        }

        private ref nint GetMultiDimensionalArrayBounds()
        {
            return ref Unsafe.As<byte, nint>(ref Unsafe.As<RawArrayData>(this).Data);
        }

        internal unsafe static Array NewMultiDimArray(EEType* pEEType, int* pLengths, int rank)
        {
            // Calculate required size for array elements
            uint totalLength = 1;
            for (int i = 0; i < rank; i++)
            {
                totalLength *= (uint)pLengths[i];
            }

            // Allocate array - note size allocated is totalLength + array base size
            // The base size incorporates the bounds for all dimensions plus the
            // eetype pointer and the number of components
            Array result = RuntimeImports.NewArray(pEEType, (int)totalLength);

            // Setup upper bounds of dimensions as nints
            ref nint bounds = ref result.GetMultiDimensionalArrayBounds();
            for (int i = 0; i < rank; i++)
            {
                Unsafe.Add(ref bounds, i) = (nint)pLengths[i];
            }

            return result;
        }

        internal static unsafe Array Ctor(EEType* pEEType, int nDimensions, int* pDimensions)
        {
            return NewMultiDimArray(pEEType, pDimensions, nDimensions);
        }

        public int GetLength(int dimension) => GetUpperBound(dimension) + 1;

        public int GetUpperBound(int dimension)
        {
            //if (!IsSzArray)
            {
                ref nint bounds = ref GetMultiDimensionalArrayBounds();
                return (int)(Unsafe.Add(ref bounds, dimension) - 1);
            }

            //return Length - 1;
        }
    }

    public class Array<T> : Array, IEnumerable<T>
    {
        public new IEnumerator<T> GetEnumerator()
        {
            T[] @this = Unsafe.As<T[]>(this);
            int length = @this.Length;

            return new SZGenericArrayEnumerator<T>(@this, length);
        }
    }

    internal sealed class SZGenericArrayEnumerator<T> : IEnumerator<T>
    {
        private readonly T[]? _array;
        private int _index;
        private readonly int _endIndex;

        internal SZGenericArrayEnumerator(T[]? array, int endIndex)
        {
            _index = -1;
            _endIndex = endIndex;
            _array = array;
        }

        public T Current => _array![_index];

        object IEnumerator.Current => Current;

        public void Dispose()
        {
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

        public void Reset() => _index = -1;
    }
}
