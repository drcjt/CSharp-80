.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.class public _xor
{
.method public static int32 Main() {
.entrypoint
.maxstack  10

	ldc.i4 0x00000000
	ldc.i4 0x00000000
	xor
	ldc.i4 0x00000000
	ceq
	brfalse FAIL

	ldc.i4 0x00000000
	ldc.i4 0x00000001
	xor
	ldc.i4 0x00000001
	ceq
	brfalse FAIL

	ldc.i4 0x00000000
	ldc.i4 0x80000000
	xor
	ldc.i4 0x80000000
	ceq
	brfalse FAIL

	ldc.i4 0x00000000
	ldc.i4 0x55555555
	xor
	ldc.i4 0x55555555
	ceq
	brfalse FAIL

	ldc.i4 0x00000000
	ldc.i4 0xAAAAAAAA
	xor
	ldc.i4 0xAAAAAAAA
	ceq
	brfalse FAIL

	ldc.i4 0x00000000
	ldc.i4 0xFFFFFFFF
	xor
	ldc.i4 0xFFFFFFFF
	ceq
	brfalse FAIL

	ldc.i4 0x00000001
	ldc.i4 0x80000000
	xor
	ldc.i4 0x80000001
	ceq
	brfalse FAIL

	ldc.i4 0x00000001
	ldc.i4 0x00000001
	xor
	ldc.i4 0x00000000
	ceq
	brfalse FAIL

	ldc.i4 0x00000001
	ldc.i4 0x80000000
	xor
	ldc.i4 0x80000001
	ceq
	brfalse FAIL

	ldc.i4 0x00000001
	ldc.i4 0x55555555
	xor
	ldc.i4 0x55555554
	ceq
	brfalse FAIL

	ldc.i4 0x00000001
	ldc.i4 0xAAAAAAAA
	xor
	ldc.i4 0xAAAAAAAB
	ceq
	brfalse FAIL

	ldc.i4 0x00000001
	ldc.i4 0xFFFFFFFF
	xor
	ldc.i4 0xFFFFFFFE
	ceq
	brfalse FAIL

	ldc.i4 0x80000000
	ldc.i4 0x00000000
	xor
	ldc.i4 0x80000000
	ceq
	brfalse FAIL

	ldc.i4 0x80000000
	ldc.i4 0x00000001
	xor
	ldc.i4 0x80000001
	ceq
	brfalse FAIL

	ldc.i4 0x80000000
	ldc.i4 0x80000000
	xor
	ldc.i4 0x00000000
	ceq
	brfalse FAIL

	ldc.i4 0x80000000
	ldc.i4 0x55555555
	xor
	ldc.i4 0xD5555555
	ceq
	brfalse FAIL

	ldc.i4 0x80000000
	ldc.i4 0xAAAAAAAA
	xor
	ldc.i4 0x2AAAAAAA
	ceq
	brfalse FAIL

	ldc.i4 0x80000000
	ldc.i4 0xFFFFFFFF
	xor
	ldc.i4 0x7FFFFFFF
	ceq
	brfalse FAIL

	ldc.i4 0x55555555
	ldc.i4 0x00000000
	xor
	ldc.i4 0x55555555
	ceq
	brfalse FAIL

	ldc.i4 0x55555555
	ldc.i4 0x00000001
	xor
	ldc.i4 0x55555554
	ceq
	brfalse FAIL

	ldc.i4 0x55555555
	ldc.i4 0x80000000
	xor
	ldc.i4 0xD5555555
	ceq
	brfalse FAIL

	ldc.i4 0x55555555
	ldc.i4 0x55555555
	xor
	ldc.i4 0x00000000
	ceq
	brfalse FAIL

	ldc.i4 0x55555555
	ldc.i4 0xAAAAAAAA
	xor
	ldc.i4 0xFFFFFFFF
	ceq
	brfalse FAIL

	ldc.i4 0x55555555
	ldc.i4 0xFFFFFFFF
	xor
	ldc.i4 0xAAAAAAAA
	ceq
	brfalse FAIL

	ldc.i4 0xAAAAAAAA
	ldc.i4 0x00000000
	xor
	ldc.i4 0xAAAAAAAA
	ceq
	brfalse FAIL

	ldc.i4 0xAAAAAAAA
	ldc.i4 0x00000001
	xor
	ldc.i4 0xAAAAAAAB
	ceq
	brfalse FAIL

	ldc.i4 0xAAAAAAAA
	ldc.i4 0x80000000
	xor
	ldc.i4 0x2AAAAAAA
	ceq
	brfalse FAIL

	ldc.i4 0xAAAAAAAA
	ldc.i4 0x55555555
	xor
	ldc.i4 0xFFFFFFFF
	ceq
	brfalse FAIL

	ldc.i4 0xAAAAAAAA
	ldc.i4 0xAAAAAAAA
	xor
	ldc.i4 0x00000000
	ceq
	brfalse FAIL

	ldc.i4 0xAAAAAAAA
	ldc.i4 0xFFFFFFFF
	xor
	ldc.i4 0x55555555
	ceq
	brfalse FAIL

	ldc.i4 0xFFFFFFFF
	ldc.i4 0x00000000
	xor
	ldc.i4 0xFFFFFFFF
	ceq
	brfalse FAIL

	ldc.i4 0xFFFFFFFF
	ldc.i4 0x00000001
	xor
	ldc.i4 0xFFFFFFFE
	ceq
	brfalse FAIL

	ldc.i4 0xFFFFFFFF
	ldc.i4 0x80000000
	xor
	ldc.i4 0x7FFFFFFF
	ceq
	brfalse FAIL

	ldc.i4 0xFFFFFFFF
	ldc.i4 0x55555555
	xor
	ldc.i4 0xAAAAAAAA
	ceq
	brfalse FAIL

	ldc.i4 0xFFFFFFFF
	ldc.i4 0xAAAAAAAA
	xor
	ldc.i4 0x55555555
	ceq
	brfalse FAIL

	ldc.i4 0xFFFFFFFF
	ldc.i4 0xFFFFFFFF
	xor
	ldc.i4 0x00000000
	ceq
	brfalse FAIL

PASS:
	ldc.i4 0x0000
	ret
FAIL:
	ldc.i4 0x0001
	ret
}
}