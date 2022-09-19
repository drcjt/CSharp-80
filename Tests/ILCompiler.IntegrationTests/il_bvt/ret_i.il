.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.class public _ret {
.method public int32& ret_test(int32&) {
.maxstack 1
	ldarg 1
	ret
}

.method public void _ret() {
.maxstack 0
	ret
}

.method public static int32 Main() {
.entrypoint
.locals(class _ret, int32)
.maxstack 5
	newobj instance void _ret::_ret()
	stloc 0
	ldc.i4 0xFF
	stloc 1
	ldloc 0
	ldloca 1
	call instance int32& _ret::ret_test(int32&)
	ldloca 1
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