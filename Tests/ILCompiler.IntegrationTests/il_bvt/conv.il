.assembly convexe
{
}
.class public conv {
.method public static int32 i4(int32) {
.maxstack 2
	ldarg 0
	ret
}
}
.method public static int32 Main() {
.entrypoint
.maxstack 10
	ldc.i4 0x8F0F0F0F
	call int32 conv::i4(int32)
	conv.i2
	ldc.i4 0x00000F0F
	ceq
	brfalse fail
pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}