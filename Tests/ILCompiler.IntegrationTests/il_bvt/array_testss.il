.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.class public array_tests
{
.method public static int32 Main() {
.entrypoint
.maxstack  5
.locals (int32[])
	ldc.i4 0x00000004
	newarr [mscorlib]System.Int32
	stloc 0

	ldloc 0
	ldc.i4 0x00
	ldc.i4 0x25
	stelem.i4

	ldloc 0
	ldc.i4 0x00
	ldelem.i4

	ldc.i4 0x25
	ceq
	brfalse FAIL

	ldloc 0
	ldc.i4 0x01
	ldc.i4 0x08
	stelem.i4

	ldloc 0
	ldc.i4 0x01
	ldelem.i4

	ldc.i4 0x08
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