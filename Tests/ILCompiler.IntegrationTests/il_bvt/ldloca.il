.assembly extern mscorlib
{
    .ver 4:0:0:0
    .publickeytoken = (B7 7A 5C 56 19 34 E0 89)
}
.class _ldloca {
.method public static int32 Main() {
.entrypoint
.maxstack 10
.locals (int32, int32&)
	ldloca	0
	stloc	1

	ldc.i4	0xFF
	stloc	0
	ldloc	1
	ldind.i4
	ldc.i4	0xFF
	ceq
	brfalse fail

	ldc.i4	0xAA
	stloc	0
	ldloc	1
	ldind.i4
	ldc.i4	0xAA
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