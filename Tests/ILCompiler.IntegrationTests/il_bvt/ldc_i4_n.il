.assembly extern mscorlib
{
    .ver 4:0:0:0
    .publickeytoken = (B7 7A 5C 56 19 34 E0 89)
}
.class ldc_i4_n {
.method public static int32 Main() {
.entrypoint
.maxstack 10
	ldc.i4.0
	ldc.i4 0
	ceq
	brfalse fail
	ldc.i4.1
	ldc.i4 1
	ceq
	brfalse fail
	ldc.i4.2
	ldc.i4 2
	ceq
	brfalse fail
	ldc.i4.3
	ldc.i4 3
	ceq
	brfalse fail
	ldc.i4.4
	ldc.i4 4
	ceq
	brfalse fail
	ldc.i4.5
	ldc.i4 5
	ceq
	brfalse fail
	ldc.i4.6
	ldc.i4 6
	ceq
	brfalse fail
	ldc.i4.7
	ldc.i4 7
	ceq
	brfalse fail
	ldc.i4.8
	ldc.i4 8
	ceq
	brfalse fail
	ldc.i4.0
	ldc.i4.1
	sub
	ldc.i4.m1
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