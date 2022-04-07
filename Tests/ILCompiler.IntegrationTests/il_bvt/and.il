.assembly extern mscorlib
{
    .ver 4:0:0:0
    .publickeytoken = (B7 7A 5C 56 19 34 E0 89)
}
.assembly andexe
{
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