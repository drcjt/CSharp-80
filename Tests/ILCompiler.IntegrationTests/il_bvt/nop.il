.assembly extern mscorlib
{
    .ver 4:0:0:0
    .publickeytoken = (B7 7A 5C 56 19 34 E0 89)
}
.assembly nopexe
{
}
.class public _nop {
.method public static int32 return(int32) {
.maxstack 2
	ldarg 0
	ret
}
.method public static int32 Main() {
.entrypoint
.maxstack 10
	ldc.i4 0xFF
	call int32 _nop::return(int32)
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	nop
	ldc.i4 0xFF
	call int32 _nop::return(int32)
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