.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.class public _cgt {

.method public static int32 Main() {
.entrypoint
.maxstack 10
	ldc.i4 0x7FFFFFFF
	ldc.i4 0x00000001
	cgt
	brfalse fail

	ldc.i4 0xFFFFFFFF
	ldc.i4 0x00000001
	cgt.un
	brfalse fail

pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}
}