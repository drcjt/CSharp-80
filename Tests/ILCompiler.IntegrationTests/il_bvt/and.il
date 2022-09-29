.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
	.ver 0:0:0:0
}
.class public _and
{
.method public static int32 Main() {
.entrypoint
.maxstack  10

	ldc.i4 0xAAAAAAAA
	ldc.i4 0xFFFFFFFF
	and
	ldc.i4 0xAAAAAAAA
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