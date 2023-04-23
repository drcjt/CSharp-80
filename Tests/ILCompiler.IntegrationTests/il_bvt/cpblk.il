.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}

.class _cpblk {

.field public static int32 DATA
.field public static int32 DATA2

.method public static int32 Main() {
.entrypoint
.maxstack 10

	ldsflda	int32 _cpblk::DATA
	ldc.i4 0xAA
	ldc.i4 4
	initblk

	ldsflda	int32 _cpblk::DATA2
	ldc.i4 0xFF
	ldc.i4 4
	initblk

	ldsflda int32 _cpblk::DATA2
	ldind.i4
	ldc.i4 0xFFFFFFFF
	ceq
	brfalse fail

	ldsflda int32 _cpblk::DATA2
	ldsflda int32 _cpblk::DATA
	ldc.i4 4
	cpblk

	ldsflda int32 _cpblk::DATA2
	ldind.i4
	ldc.i4 0xAAAAAAAA
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