.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.class public _neg {

.method public static int32 neg_i(int32, int32) {
.maxstack 2
	ldarg 0
    conv.i
	neg
    ldarg 1
    conv.i
    ceq
	ret
}

.method public static int32 Main() {
.entrypoint
.maxstack 10
    // neg(-32768) = -32768
	ldc.i4 0x00008000
	ldc.i4 0x00008000
	call int32 _neg::neg_i(int32,int32)
    brfalse fail

    // neg(-32767) = 32767
    ldc.i4 0x00008001
	ldc.i4 0x00007FFF
	call int32 _neg::neg_i(int32,int32)
    brfalse fail

    // neg(-1) = +1
	ldc.i4 0x0000FFFF
	ldc.i4 0x00000001
	call int32 _neg::neg_i(int32,int32)
    brfalse fail

    // neg(0) = 0
    ldc.i4 0x00000000
	ldc.i4 0x00000000
	call int32 _neg::neg_i(int32,int32)
    brfalse fail

    // neg(1) = -1
    ldc.i4 0x00000001
	ldc.i4 0x0000FFFF
	call int32 _neg::neg_i(int32,int32)
    brfalse fail

    // neg(32766) = -32766
	ldc.i4 0x00007FFE
	ldc.i4 0x00008002
	call int32 _neg::neg_i(int32,int32)
    brfalse fail

    // neg(32767) = -32767
	ldc.i4 0x00007FFF
	ldc.i4 0x00008001
	call int32 _neg::neg_i(int32,int32)
    brfalse fail

    // neg(21845) = -21845
	ldc.i4 0x00005555
	ldc.i4 0x0000AAAB
	call int32 _neg::neg_i(int32,int32)
    brfalse fail

    // neg(-21846) = +21846
	ldc.i4 0x0000AAAA
	ldc.i4 0x00005556
	call int32 _neg::neg_i(int32,int32)
    brfalse fail

pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}
}
