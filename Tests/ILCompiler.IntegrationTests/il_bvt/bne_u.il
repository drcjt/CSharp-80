.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}

.method public static int32 Main() {
.entrypoint
.maxstack 10
.locals (int32, int32*, int32*)

	ldloca	0
	stloc 1
	ldloca	0
	ldc.i4 0x2
	add
	stloc 2

	ldloc 1
	ldloc 2
	bne.un A
	br fail

A:
	ldloc 2
	ldloc 1
	bne.un B
	br fail

B:
	ldloc 1
	ldloc 1
	bne.un fail

pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}