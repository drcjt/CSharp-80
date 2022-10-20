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

	ldc.i4 0xFF
	newarr [mscorlib]System.Int32
	ldlen
	ldc.i4 0xFF
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