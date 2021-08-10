.assembly extern mscorlib
{
    .ver 4:0:0:0
    .publickeytoken = (B7 7A 5C 56 19 34 E0 89)
}
.class ldarg_starg {
.method public static int32 stargi4(int32) {
.maxstack 5
	ldarg 0
	/*TODO: Uncomment these instructions when starg is implemented
	starg 0
	ldarg 0
	*/
	ret
}
.method public static int32 Main() {
.entrypoint
.maxstack 10
	ldc.i4 0x1
	call int32 ldarg_starg::stargi4(int32)
	ldc.i4 0x1
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