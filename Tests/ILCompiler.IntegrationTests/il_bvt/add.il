.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}

.assembly TestAssembly
{
	.ver 0:0:0:0
}
.class public _add
{
.method public static int32 Main() {
.entrypoint
.maxstack  5
.locals ()
	ldc.i4 0xAABB
	conv.i2
	ldc.i4 0x1111
	add
	ldc.i4 0xFFFFBBCC
	ceq
	brfalse FAIL
PASS:
	ldc.i4 0x0000
	ret
FAIL:
	ldc.i4 0x0001
	ret
}
}