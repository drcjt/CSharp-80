.assembly beq_i
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
	beq fail

	ldloc 2
	ldloc 1
	beq fail

	ldloc 1
	ldloc 1
	beq pass
	br fail

pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}