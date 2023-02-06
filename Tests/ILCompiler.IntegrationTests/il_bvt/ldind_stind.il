.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}

.class ldind_stind {
.method public static int32 Main() {
.entrypoint
.maxstack 10
.locals (int8, int16, int32, int32*)
	ldloca	0
	ldc.i4 0x00000011
	stind.i1

	ldloca 0
	ldind.u1
	ldc.i4 0x00000011
	ceq
	brfalse fail

	ldloca	0
	ldc.i4 0x00000011
	stind.i1

	ldloca 0
	ldind.i1
	ldc.i4 0x00000011
	ceq
	brfalse fail

	ldloca 1
	ldc.i4 0x00002222
	stind.i2

	ldloca 1
	ldind.u2
	ldc.i4 0x00002222
	ceq
	brfalse fail

	ldloca 1
	ldind.i2
	ldc.i4 0x00002222
	ceq
	brfalse fail
	
	ldloca 2
	ldc.i4 0x44444444
	stind.i4

	ldloca 2
	ldind.u4
	ldc.i4 0x44444444
	ceq
	brfalse fail

	ldloca 2
	ldind.i4
	ldc.i4 0x44444444
	ceq
	brfalse fail

	ldloca 3
	ldloca 2
	stind.i

	ldloca 3
	ldind.i
	ldc.i4 0x55555555
	stind.i4

	ldloca 2
	ldind.i4
	ldc.i4 0x55555555
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