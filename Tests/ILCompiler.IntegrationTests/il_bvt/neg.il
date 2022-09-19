.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.class public _neg {
.method public static int32 negi4(int32) {
.maxstack 2
	ldarg 0
	neg
	ret
}
.method public static int32 Main() {
.entrypoint
.maxstack 10
	ldc.i4 0x00000002
	call int32 _neg::negi4(int32)
	ldc.i4 0xFFFFFFFE
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