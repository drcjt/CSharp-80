.assembly extern mscorlib
{
    .ver 4:0:0:0
    .publickeytoken = (B7 7A 5C 56 19 34 E0 89)
}
.class ldind_stind {
.method public static int32 Main() {
.entrypoint
.maxstack 10
.locals (int8, int16, int32)
	ldloca	0
	ldc.i4 0x00000011
	stind.i1

	ldloca 1
	ldc.i4 0x00002222
	stind.i2

	ldloca 2
	ldc.i4 0x44444444
	stind.i4

	ldloca 0
	ldind.i1
	ldc.i4 0x00000011
	ceq
	brfalse fail

	ldloca 1
	ldind.i2
	ldc.i4 0x00002222
	ceq
	brfalse fail

	ldloca 2
	ldind.i4
	ldc.i4 0x44444444
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