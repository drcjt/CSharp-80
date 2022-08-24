.assembly extern mscorlib
{
    .ver 4:0:0:0
    .publickeytoken = (B7 7A 5C 56 19 34 E0 89)
}
.class ldloc_stloc {
.method public static int32 Main() {
.entrypoint
.maxstack 10
.locals (int32, int16, int8)

	ldc.i4	0x7FFFFFFF
	stloc	0
	ldc.i4	0x7FFFFFFF
	ldloc	0
	ceq
	brfalse fail

	ldc.i4  0x1
	stloc   1
	ldc.i4  0x1
	ldloc   1
	ceq
	brfalse fail

pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}
}