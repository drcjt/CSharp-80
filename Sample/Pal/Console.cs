namespace System
{
	public class Console
	{

		[System.Runtime.CompilerServices.Intrinsic]
		public static void Write(char value) { }
		[System.Runtime.CompilerServices.Intrinsic]
		public static void Write(int value) { }
		[System.Runtime.CompilerServices.Intrinsic]
		public static void Write(string str) { }

		// Test of using dll import to link to native assembly
		// TODO: Consider if this is better/worse than either intrinsics or
		// perhaps using MethodImplOptions.InternalCall
		[System.Runtime.InteropServices.DllImport("Runtime", EntryPoint = "WRITE")]
		public static unsafe extern bool WriteCh(char ch);

		public static void WriteLine(string str) { Write(str); Write('\n'); }
	}
}
