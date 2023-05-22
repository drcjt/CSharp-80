.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.method public static int32 Main() {
.entrypoint
.locals (int32*, int32*)

	ldloc 0
	ldc.i4 0x2
	add
	stloc 1

	ldloc 0
	ldloc 1
	bge fail

	ldloc 1
	ldloc 0
	bge next
	br fail

next:
	ldloc 0
	ldloc 0
	bge pass
	br fail

pass:
	ldc.i4 0x0000
	br end
fail:
	ldc.i4 0x0001
	br end
end:
	ret
}