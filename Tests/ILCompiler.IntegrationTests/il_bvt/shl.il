.assembly extern mscorlib
{
    .ver 4:0:0:0
    .publickeytoken = (B7 7A 5C 56 19 34 E0 89)
}
.assembly shlexe
{
}
.class public _shl
{
.method public static int32 Main() {
.entrypoint
.maxstack  10
.locals ()
	ldc.i4 0x7FFFFFFF
	ldc.i4 0x1
	shl
	ldc.i4 0xFFFFFFFE
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