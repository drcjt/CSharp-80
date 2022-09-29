.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.class public _or
{
.method public static int32 Main() {
.entrypoint
.maxstack  10

	ldc.i4 0xCCCCCCCC
	ldc.i4 0xF0F0F0F0
	or
	ldc.i4 0xFCFCFCFC
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