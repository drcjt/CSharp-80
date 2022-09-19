.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.method public static int32 Main() {
.entrypoint
.locals ()
	ldc.i4 0x0
	brfalse pass
fail:
	ldc.i4 0x0001
	ret
pass:
	ldc.i4 0x0000
	ret
}