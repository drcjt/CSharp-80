.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.class public _dup {
.method public static int32 _dup(int32) {
.maxstack 2
	ldarg 0
	dup
	ceq
	ret
}
.method public static int32 Main() {
.entrypoint
.maxstack 10
	ldc.i4 0xFAFAFAFA
	call int32 _dup::_dup(int32)
	brfalse fail
pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}
}