.assembly extern mscorlib
{
    .ver 4:0:0:0
    .publickeytoken = (B7 7A 5C 56 19 34 E0 89)
}
.assembly shlexe
{
}
.class public Shl_I4
{
.method public static int32 _shl(int32,int32,int32) {
.maxstack 20
	ldarg 0
	ldarg 1
	shl
	ldarg 2
	ceq
	ret
}


.method public static int32 Main() {
.entrypoint
.maxstack  10
.locals ()

	ldc.i4 0x00000000
	ldc.i4 0x00000000
	ldc.i4 0x00000000
	call int32 Shl_I4::_shl(int32,int32,int32)
	brfalse FAIL

	ldc.i4 0x00000000
	ldc.i4 0x00000001
	ldc.i4 0x00000000
	call int32 Shl_I4::_shl(int32,int32,int32)
	brfalse FAIL

	ldc.i4 0x00000000
	ldc.i4 0x0000001F
	ldc.i4 0x00000000
	call int32 Shl_I4::_shl(int32,int32,int32)
	brfalse FAIL

	ldc.i4 0xFFFFFFFF
	ldc.i4 0x00000000
	ldc.i4 0xFFFFFFFF
	call int32 Shl_I4::_shl(int32,int32,int32)
	brfalse FAIL

	ldc.i4 0xFFFFFFFF
	ldc.i4 0x00000001
	ldc.i4 0xFFFFFFFE
	call int32 Shl_I4::_shl(int32,int32,int32)
	brfalse FAIL

	ldc.i4 0xFFFFFFFF
	ldc.i4 0x0000001F
	ldc.i4 0x80000000
	call int32 Shl_I4::_shl(int32,int32,int32)
	brfalse FAIL
	
	ldc.i4 0x6789ABCD
	ldc.i4 0x00000000
	ldc.i4 0x6789ABCD
	call int32 Shl_I4::_shl(int32,int32,int32)
	brfalse FAIL

	ldc.i4 0x6789ABCD
	ldc.i4 0x00000001
	ldc.i4 0xCF13579A
	call int32 Shl_I4::_shl(int32,int32,int32)
	brfalse FAIL

	ldc.i4 0x6789ABCD
	ldc.i4 0x0000001F
	ldc.i4 0x80000000
	call int32 Shl_I4::_shl(int32,int32,int32)
	brfalse FAIL

PASS:
	ldc.i4 0x0000
	ret
FAIL:
	ldc.i4 0x0001
	ret
}
}