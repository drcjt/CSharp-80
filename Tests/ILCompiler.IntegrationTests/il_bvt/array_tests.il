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
.locals (int32[], int8[], int16[])
	ldc.i4 0x00000004
	newarr [mscorlib]System.Int32
	stloc 0

	ldloc 0
	ldc.i4 0x00
	ldc.i4 0x7fffffff
	stelem.i4

	ldloc 0
	ldc.i4 0x00
	ldelem.i4

	ldc.i4 0x7fffffff
	ceq
	brfalse FAIL

	ldloc 0
	ldc.i4 0x01
	ldc.i4 0x80000000
	stelem.i4

	ldloc 0
	ldc.i4 0x01
	ldelem.i4

	ldc.i4 0x80000000
	ceq
	brfalse FAIL

	ldc.i4 0x00000004
	newarr [mscorlib]System.Byte
	stloc 1

	ldloc 1
	ldc.i4 0x00
	ldc.i4 0x25
	stelem.i1

	ldloc 1
	ldc.i4 0x00
	ldelem.i1

	ldc.i4 0x25
	ceq
	brfalse FAIL

	ldloc 1
	ldc.i4 0x01
	ldc.i4 0x08
	stelem.i1

	ldloc 1
	ldc.i4 0x01
	ldelem.i1

	ldc.i4 0x08
	ceq
	brfalse FAIL

	ldc.i4 0x00000004
	newarr [mscorlib]System.Int16
	stloc 2

	ldloc 2
	ldc.i4 0x00
	ldc.i4 0x00007fff
	stelem.i2

	ldloc 2
	ldc.i4 0x00
	ldelem.i2

	ldc.i4 0x00007fff
	ceq
	brfalse FAIL


	ldloc 2
	ldc.i4 0x01
	ldc.i4 0x0000ffff
	stelem.i2

	ldloc 2
	ldc.i4 0x01
	ldelem.i2

	ldc.i4 0xffffffff	// negative value is sign extended
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