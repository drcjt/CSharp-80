.assembly extern mscorlib
{
    .ver 4:0:0:0
    .publickeytoken = (B7 7A 5C 56 19 34 E0 89)
}
.assembly array_tests
{
}
.class public array_tests
{
.method public static int32 Main() {
.entrypoint
.maxstack  5
.locals ()
	ldc.i4 0x00000004
	newarr [mscorlib]System.Int32

PASS:
	ldc.i4 0x0000
	ret
FAIL:
	ldc.i4 0x0001
	ret
}
}