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