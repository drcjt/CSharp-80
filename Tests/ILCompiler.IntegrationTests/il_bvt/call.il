.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.method public static int32 test() {
	ldc.i4 0xFF
	ret
}
.method public static int32 Main() {
.entrypoint
.maxstack 10
	call int32 test()
	ldc.i4 0xFF
	ceq
	brfalse fail
pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}