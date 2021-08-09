.assembly remexe
{
}
.class public _rem {
.method public static int32 I4(int32,int32,int32) {
.maxstack 3
	ldarg 0
	ldarg 1
	rem
	ldarg 2
	ceq
	ret
}
.method public static int32 U4(int32,int32,int32) {
.maxstack 3
	ldarg 0
	ldarg 1
	rem.un
	ldarg 2
	ceq
	ret
}
.method public static int32 Main() {
.entrypoint
.maxstack 10
	ldc.i4 10
	ldc.i4 4
	ldc.i4 2
	call int32 _rem::I4(int32,int32,int32)
	brfalse fail
	ldc.i4 10
	ldc.i4 4
	ldc.i4 2
	call int32 _rem::U4(int32,int32,int32)
	brfalse fail
pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}
}