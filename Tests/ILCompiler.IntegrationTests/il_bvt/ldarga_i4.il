.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.class _ldarga {
.method public static int32 args(int32) {
.maxstack 5
	ldarga 0
	ldind.i4
	ldc.i4 0x2
	add
	ret
}

.method public static int32 Main() {
.entrypoint
.maxstack 10
	ldc.i4 0x80000002
	ldc.i4 0x80000000
	call int32 _ldarga::args(int32)
	ceq
	brfalse fail

	ldc.i4 0x00000001
	ldc.i4 0xFFFFFFFF
	call int32 _ldarga::args(int32)
	ceq
	brfalse fail

	ldc.i4 0x00000002
	ldc.i4 0x00000000
	call int32 _ldarga::args(int32)
	ceq
	brfalse fail

	ldc.i4 0x00000003
	ldc.i4 0x00000001
	call int32 _ldarga::args(int32)
	ceq
	brfalse fail

	ldc.i4 0x80000001
	ldc.i4 0x7FFFFFFF
	call int32 _ldarga::args(int32)
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