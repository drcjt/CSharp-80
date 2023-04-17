.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}

.class _initblk {

.field public static int32 DATA

.method public static int32 Main() {
.entrypoint
.maxstack 10
	ldsflda	int32 _initblk::DATA
	ldc.i4 0xAA
	ldc.i4 4
	initblk

	ldsflda int32 _initblk::DATA
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