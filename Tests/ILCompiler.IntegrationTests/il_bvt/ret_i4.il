.assembly extern mscorlib
{
    .ver 4:0:0:0
    .publickeytoken = (B7 7A 5C 56 19 34 E0 89)
}
.assembly ret_i4
{
}
.class public _ret {
.method public static int32 _ret_test(int32) {
.maxstack 5
	ldarg 0
	ret
}
.method public static int32 Main() {
.entrypoint
.maxstack 10
	ldc.i4 0xAAAAAAAA
	call int32 _ret::_ret_test(int32)
	ldc.i4 0xAAAAAAAA
	ceq
	brfalse fail

	ldc.i4 0x55555555
	call int32 _ret::_ret_test(int32)
	ldc.i4 0x55555555
	ceq
	brfalse fail

	ldc.i4 0x7FFFFFFF
	call int32 _ret::_ret_test(int32)
	ldc.i4 0x7FFFFFFF
	ceq
	brfalse fail

	ldc.i4 0x00000001
	call int32 _ret::_ret_test(int32)
	ldc.i4 0x00000001
	ceq
	brfalse fail

	ldc.i4 0x00000000
	call int32 _ret::_ret_test(int32)
	ldc.i4 0x00000000
	ceq
	brfalse fail

	ldc.i4 0xFFFFFFFF
	call int32 _ret::_ret_test(int32)
	ldc.i4 0xFFFFFFFF
	ceq
	brfalse fail

	ldc.i4 0x80000000
	call int32 _ret::_ret_test(int32)
	ldc.i4 0x80000000
	ceq
	brfalse fail

pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}
}