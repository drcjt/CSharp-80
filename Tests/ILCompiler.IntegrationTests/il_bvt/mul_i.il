.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.class public _mul {

.method public static int32 Main() {
.entrypoint
.maxstack 10

	ldc.i4.1
	conv.i
	ldc.i4.4
	mul

	ldc.i4.4
	conv.i

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