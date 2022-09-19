.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.class public _div {
.method public static int32 i4(int32,int32) {
.maxstack 2
	ldarg 0
	ldarg 1
	div
	ret
}
.method public static int32 u4(int32,int32) {
.maxstack 2
	ldarg 0
	ldarg 1
	div.un
	ret
}
.method public static int32 Main() {
.entrypoint
.maxstack 10
	ldc.i4 0xF0F0F0F0
	ldc.i4 0x01010101
	call int32 _div::i4(int32,int32)
	ldc.i4 0xFFFFFFF1
	ceq
	brfalse fail
	ldc.i4 0xFFFFFFFF
	ldc.i4 0x00000002
	call int32 _div::u4(int32,int32)
	ldc.i4 0x7FFFFFFF
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