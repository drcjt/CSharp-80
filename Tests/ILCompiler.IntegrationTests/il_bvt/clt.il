.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.class public _clt {

.method public static int32 Main() {
.entrypoint
.maxstack 10
	ldc.i4 0x00000001
	ldc.i4 0x7FFFFFFF
	clt
	brfalse fail

	ldc.i4 0x00000001
	ldc.i4 0xFFFFFFFF
	clt.un
	brfalse fail

	ldc.i4 0x00000001
	conv.u
	ldc.i4 0xFFFFFFFF
	conv.u
	clt.un
	brfalse fail

pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}
}