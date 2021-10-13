.assembly dupexe
{
}
.assembly extern mscorlib
{
    .ver 4:0:0:0
    .publickeytoken = (B7 7A 5C 56 19 34 E0 89)
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