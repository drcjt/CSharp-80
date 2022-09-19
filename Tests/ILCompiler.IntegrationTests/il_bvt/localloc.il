.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.class public _localloc {
.method public static int32 Main() {
.entrypoint
.maxstack 10
.locals (native int)

	ldc.i4 0x1
	localloc
	stloc 0
	ldloc 0
	ldc.i4 0x8
	conv.i
	stind.i
	ldloc 0
	ldind.i
	ldc.i4 0x8
	conv.i
	bne.un fail


	ldc.i4	0x1
	localloc
	stloc 0
	ldloc 0
	ldc.i4 0x99
	// TODO: Uncomment when conv.i1 implemented
	//conv.i1
	stind.i1
	ldloc.0
	ldind.i1
	ldc.i4 0x99

	// TODO: Uncomment when conv.i1 implemented
	//conv.i1
	bne.un fail
	ldc.i4	0x2
	localloc
	stloc 0
	ldloc 0
	ldc.i4 100
	conv.i2
	stind.i2
	ldloc.0
	ldind.i2
	ldc.i4 100
	conv.i2
	bne.un fail

	ldc.i4	0x4
	localloc
	stloc 0
	ldloc 0
	ldc.i4 0xBBBBBBBB
	stind.i4
	ldloc.0
	ldind.i4
	ldc.i4 0xBBBBBBBB
	bne.un fail

pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}
}