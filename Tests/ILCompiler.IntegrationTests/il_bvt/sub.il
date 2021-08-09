.assembly subexe
{
}
.class public _sub {
.method public static int32 I4(int32,int32) {
.maxstack 2
	ldarg 0
	ldarg 1
	sub
	ret
}
.method public static int32 Main() {
.entrypoint
.maxstack 10
	ldc.i4 0xFF
	ldc.i4 0xF0
	call int32 _sub::I4(int32,int32)
	ldc.i4 0xF
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