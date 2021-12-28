.assembly extern mscorlib
{
    .ver 4:0:0:0
    .publickeytoken = (B7 7A 5C 56 19 34 E0 89)
}
.assembly mulexe
{
}
.class public _mul {
.method public static int32 i4(int32,int32,int32) {
.maxstack 3
	ldarg 0
	ldarg 1
	mul
	ldarg 2
	ceq
	ret
}
.method public static int32 Main() {
.entrypoint
.maxstack 10

	// -2147483648 * -2147483648 = 0
	ldc.i4 0x80000000
	ldc.i4 0x80000000
	ldc.i4 0x00000000
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// -2147483648 * -1 = -2147483648
	ldc.i4 0x80000000
	ldc.i4 0xFFFFFFFF
	ldc.i4 0x80000000
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// -2147483648 * 0 = 0
	ldc.i4 0x80000000
	ldc.i4 0x00000000
	ldc.i4 0x00000000
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail
	
	// -2147483648 * 1 = -2147483648
	ldc.i4 0x80000000
	ldc.i4 0x00000001
	ldc.i4 0x80000000
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// -2147483648 * 2147483647 = -2147483648
	ldc.i4 0x80000000
	ldc.i4 0x7FFFFFFF
	ldc.i4 0x80000000
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// -2147483648 * 1431655765 = -2147483648
	ldc.i4 0x80000000
	ldc.i4 0x55555555
	ldc.i4 0x80000000
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// -2147483648 * -1431655766 = 0
	ldc.i4 0x80000000
	ldc.i4 0xAAAAAAAA
	ldc.i4 0x00000000
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// -1 * -2147483648 = -2147483648
	ldc.i4 0xFFFFFFFF
	ldc.i4 0x80000000
	ldc.i4 0x80000000
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// -1 * -1 = 1
	ldc.i4 0xFFFFFFFF
	ldc.i4 0xFFFFFFFF
	ldc.i4 0x00000001
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// -1 * 0 = 0
	ldc.i4 0xFFFFFFFF
	ldc.i4 0x00000000
	ldc.i4 0x00000000
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// -1 * 1 = -1
	ldc.i4 0xFFFFFFFF
	ldc.i4 0x00000001
	ldc.i4 0xFFFFFFFF
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// -1 * 2147483647 = -2147483647
	ldc.i4 0xFFFFFFFF
	ldc.i4 0x7FFFFFFF
	ldc.i4 0x80000001
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// -1 * 1431655765 = -1431655765
	ldc.i4 0xFFFFFFFF
	ldc.i4 0x55555555
	ldc.i4 0xAAAAAAAB
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// -1 * -1431655766 = 1431655766
	ldc.i4 0xFFFFFFFF
	ldc.i4 0xAAAAAAAA
	ldc.i4 0x55555556
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// 0 * -2147483648 = 0
	ldc.i4 0x00000000
	ldc.i4 0x80000000
	ldc.i4 0x00000000
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// 0 * -1 = 0
	ldc.i4 0x00000000
	ldc.i4 0xFFFFFFFF
	ldc.i4 0x00000000
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// 0 * 0 = 0
	ldc.i4 0x00000000
	ldc.i4 0x00000000
	ldc.i4 0x00000000
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// 0 * 1 = 0
	ldc.i4 0x00000000
	ldc.i4 0x00000001
	ldc.i4 0x00000000
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// 0 * 2147483647 = 0
	ldc.i4 0x00000000
	ldc.i4 0x7FFFFFFF
	ldc.i4 0x00000000
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// 0 * 1431655765 = 0
	ldc.i4 0x00000000
	ldc.i4 0x55555555
	ldc.i4 0x00000000
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// 0 * -1431655766 = 0
	ldc.i4 0x00000000
	ldc.i4 0xAAAAAAAA
	ldc.i4 0x00000000
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// 1 * -1 = -1
	ldc.i4 0x00000001
	ldc.i4 0x80000000
	ldc.i4 0x80000000
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// 1 * -1 = -1
	ldc.i4 0x00000001
	ldc.i4 0xFFFFFFFF
	ldc.i4 0xFFFFFFFF
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// 1 * 0 = 0
	ldc.i4 0x00000001
	ldc.i4 0x00000000
	ldc.i4 0x00000000
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// 1 * 1 = 1
	ldc.i4 0x00000001
	ldc.i4 0x00000001
	ldc.i4 0x00000001
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// 1 * 2147483647 = 2147483647
	ldc.i4 0x00000001
	ldc.i4 0x7FFFFFFF
	ldc.i4 0x7FFFFFFF
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail
	
	// 1 * 1431655765 = 1431655765
	ldc.i4 0x00000001
	ldc.i4 0x55555555
	ldc.i4 0x55555555
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// 1 * -1431655766 = -1431655766
	ldc.i4 0x00000001
	ldc.i4 0xAAAAAAAA
	ldc.i4 0xAAAAAAAA
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// 2147483647 * -2147483648 = -2147483648
	ldc.i4 0x7FFFFFFF
	ldc.i4 0x80000000
	ldc.i4 0x80000000
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// 2147483647 * -1 = -2147483647
	ldc.i4 0x7FFFFFFF
	ldc.i4 0xFFFFFFFF
	ldc.i4 0x80000001
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// 2147483647 * 0 = 0
	ldc.i4 0x7FFFFFFF
	ldc.i4 0x00000000
	ldc.i4 0x00000000
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// 2147483647 * 1 = 2147483647
	ldc.i4 0x7FFFFFFF
	ldc.i4 0x00000001
	ldc.i4 0x7FFFFFFF
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// 2147483647 * 2147483647 = 1
	ldc.i4 0x7FFFFFFF
	ldc.i4 0x7FFFFFFF
	ldc.i4 0x00000001
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// Fails here
	// 2147483647 * 1431655765 = 715827883
	ldc.i4 0x7FFFFFFF
	ldc.i4 0x55555555
	ldc.i4 0x2AAAAAAB
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// 2147483647 * -1431655766 = 1431655766
	ldc.i4 0x7FFFFFFF
	ldc.i4 0xAAAAAAAA
	ldc.i4 0x55555556
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// 1431655765 * -2147483648 = -2147483648
	ldc.i4 0x55555555
	ldc.i4 0x80000000
	ldc.i4 0x80000000
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// 1431655765 * -2147483648 = -1431655765 
	ldc.i4 0x55555555
	ldc.i4 0xFFFFFFFF
	ldc.i4 0xAAAAAAAB
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// 1431655765 * 0 = 0
	ldc.i4 0x55555555
	ldc.i4 0x00000000
	ldc.i4 0x00000000
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail
	
	// 1431655765 * 1 = 1431655765
	ldc.i4 0x55555555
	ldc.i4 0x00000001
	ldc.i4 0x55555555
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// 1431655765 * 2147483647 = 715827883
	ldc.i4 0x55555555
	ldc.i4 0x7FFFFFFF
	ldc.i4 0x2AAAAAAB
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// 1431655765 * 1431655765 = 954437177
	ldc.i4 0x55555555
	ldc.i4 0x55555555
	ldc.i4 0x38E38E39
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// 1431655765 * -1431655766 = 1908874354
	ldc.i4 0x55555555
	ldc.i4 0xAAAAAAAA
	ldc.i4 0x71C71C72
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// -1431655766 * -2147483648 = 0
	ldc.i4 0xAAAAAAAA
	ldc.i4 0x80000000
	ldc.i4 0x00000000
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// -1431655766 * -1 = 1431655766
	ldc.i4 0xAAAAAAAA
	ldc.i4 0xFFFFFFFF
	ldc.i4 0x55555556
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// -1431655766 * 0 = 0
	ldc.i4 0xAAAAAAAA
	ldc.i4 0x00000000
	ldc.i4 0x00000000
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// -1431655766 * 1 = -1431655766
	ldc.i4 0xAAAAAAAA
	ldc.i4 0x00000001
	ldc.i4 0xAAAAAAAA
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// -1431655766 * 2147483647 = 1431655766
	ldc.i4 0xAAAAAAAA
	ldc.i4 0x7FFFFFFF
	ldc.i4 0x55555556
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// -1431655766 * 1431655765 = 1908874354
	ldc.i4 0xAAAAAAAA
	ldc.i4 0x55555555
	ldc.i4 0x71C71C72
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// -1431655766 * -1431655766 = -477218588
	ldc.i4 0xAAAAAAAA
	ldc.i4 0xAAAAAAAA
	ldc.i4 0xE38E38E4
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

	// 1431655765 * 2 = -1431655766
	ldc.i4 0x55555555
	ldc.i4 0x00000002
	ldc.i4 0xAAAAAAAA
	call int32 _mul::i4(int32,int32,int32)
	brfalse fail

pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}
}