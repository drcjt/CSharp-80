.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}

.class public _ldlen
{

.method public static int32 Main() {
.entrypoint
.maxstack  5
.locals ()

	ldc.i4 0x04
	ldc.i4 0x04
	newarr [mscorlib]System.Int32
	ldlen
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