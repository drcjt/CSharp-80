.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.class public _ceq {

.method public static int32 Main() {
.entrypoint
.maxstack 10

	ldc.i4 0x1234ABCD
	ldc.i4 0x1234ABCD
	ceq
	brfalse fail

	ldc.i4 0x1234ABCD
	conv.u
	ldc.i4 0x1234ABCD
	conv.u
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