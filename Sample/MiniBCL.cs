﻿namespace System
{
	public class Object
	{
		public IntPtr m_pEEType;
	}

	public struct Void { }

	public sealed class String
	{
		public readonly int Length;
		public char _firstChar;

		public unsafe char this[int index]
		{
			[System.Runtime.CompilerServices.Intrinsic]
			get
			{
				return Internal.Runtime.CompilerServices.Unsafe.Add(ref _firstChar, index);
			}
		}
	}

	public struct Boolean { }
	public struct Char { }
	public struct Int16 { }
	public struct Int32 { }
	public struct IntPtr { }
	public struct Byte { }

	public abstract class ValueType { }
	public abstract class Enum : ValueType { }
	public class Attribute { }
}

namespace System.Runtime.CompilerServices
{
	internal sealed class IntrinsicAttribute : Attribute { }

	public class RuntimeHelpers
	{
		public static unsafe int OffsetToStringData => 0; //sizeof(IntPtr) + sizeof(int);
	}
}

namespace System.Runtime.InteropServices
{
	public enum CharSet
	{
		None = 1,
		Ansi = 2,
		Unicode = 3,
		Auto = 4,
	}

	public sealed class DllImportAttribute : Attribute
	{
		public string EntryPoint;
		public CharSet CharSet;
		public DllImportAttribute(string dllName) { }
	}

	public enum LayoutKind
	{
		Sequential = 0,
		Explicit = 2,
		Auto = 3
	}

	public sealed class StructLayoutAttribute : Attribute
	{
		public StructLayoutAttribute(LayoutKind layoutKind) { }
	}
}

namespace Internal.Runtime.CompilerServices
{
	public static unsafe partial class Unsafe
	{
		// The body of this method is generated by the compiler
		// It will do what Unsafe.Add is expected to do. It's just not possible to express it in C#
		[System.Runtime.CompilerServices.Intrinsic]
		public static extern ref T Add<T>(ref T source, int elementOffset);
	}
}