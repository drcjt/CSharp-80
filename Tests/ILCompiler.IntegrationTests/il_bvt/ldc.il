.class ldc {
.method public static int32 i4(int32) {
.maxstack 2
	ldarg 0
	ret
}
.method public static int32 Main() {
.entrypoint
.maxstack 10
	ldc.i4 0xAAAAAAAA
	call int32 ldc::i4(int32)
	ldc.i4 0xAAAAAAAA
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