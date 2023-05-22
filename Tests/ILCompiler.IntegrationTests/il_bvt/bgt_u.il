.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.method public static int32 Main() {
.entrypoint
.locals (int32, int32*, int32*)

	ldloca.s 0
	stloc 1
	ldloca.s 0
	ldc.i4 0x2
	add
	stloc 2

	ldloc 1
	ldloc 2
	bgt.un fail

	ldloc 2
	ldloc 1
	bgt.un next
	br fail

next:
	ldloc 1
	ldloc 1
	bgt.un fail

pass:
	ldc.i4 0x0000
	br end
fail:
	ldc.i4 0x0001
	br end
end:
	ret
}