.assembly extern mscorlib
{
    .ver 4:0:0:0
    .publickeytoken = (B7 7A 5C 56 19 34 E0 89)
}
.assembly mulovfexe
{
}
.class public _mul_ovf {

.method public static int32 u4(int32,int32) {
.maxstack 5
	ldarg 0
	ldarg 1
	mul.ovf.un
	//conv.ovf.u4
	ret
}

.method public static int32 Main() {
.entrypoint
.maxstack 10

	// 0 * 0 = 0
	ldc.i4 0x00000000
	ldc.i4 0x00000000
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0x00000000
	ceq
	brfalse fail

	// 0 * 1 = 0
	ldc.i4 0x00000000
	ldc.i4 0x00000001
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0x00000000
	ceq
	brfalse fail

	// 0 * 4294967294 = 0
	ldc.i4 0x00000000
	ldc.i4 0xFFFFFFFE
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0x00000000
	ceq
	brfalse fail

	// 0 * 4294967295 = 0
	ldc.i4 0x00000000
	ldc.i4 0xFFFFFFFF
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0x00000000
	ceq
	brfalse fail

	// 0 * 1431655765 = 0
	ldc.i4 0x00000000
	ldc.i4 0x55555555
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0x00000000
	ceq
	brfalse fail

	// 0 * 2863311530 = 0
	ldc.i4 0x00000000
	ldc.i4 0xAAAAAAAA
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0x00000000
	ceq
	brfalse fail

	// 1 * 0 = 0
	ldc.i4 0x00000001
	ldc.i4 0x00000000
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0x00000000
	ceq
	brfalse fail

	// 1 * 1 = 1
	ldc.i4 0x00000001
	ldc.i4 0x00000001
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0x00000001
	ceq
	brfalse fail

	// 1 * 4294967294 = 4294967294
	ldc.i4 0x00000001
	ldc.i4 0xFFFFFFFE
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0xFFFFFFFE
	ceq
	brfalse fail

	// 1 * 4294967295 = 4294967295
	ldc.i4 0x00000001
	ldc.i4 0xFFFFFFFF
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0xFFFFFFFF
	ceq
	brfalse fail

	// 1 * 1431655765 = 1431655765
	ldc.i4 0x00000001
	ldc.i4 0x55555555
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0x55555555
	ceq
	brfalse fail

	// 1 * 2863311530 = 2863311530
	ldc.i4 0x00000001
	ldc.i4 0xAAAAAAAA
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0xAAAAAAAA
	ceq
	brfalse fail

	// 4294967294 * 0 = 0
	ldc.i4 0xFFFFFFFE
	ldc.i4 0x00000000
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0x00000000
	ceq
	brfalse fail

	// 4294967294 * 1 = 4294967294
	ldc.i4 0xFFFFFFFE
	ldc.i4 0x00000001
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0xFFFFFFFE
	ceq
	brfalse fail

	/*
	// This should throw an overflow exception
	ldc.i4 0xFFFFFFFE
	ldc.i4 0xFFFFFFFE
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0xAEAEAEAE
	ceq
	brfalse fail

	// This should throw an overflow exception
	ldc.i4 0xFFFFFFFE
	ldc.i4 0xFFFFFFFF
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0xAEAEAEAE
	ceq
	brfalse fail

	// This should throw an overflow exception
	ldc.i4 0xFFFFFFFE
	ldc.i4 0x55555555
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0xAEAEAEAE
	ceq
	brfalse fail

	// This should throw an overflow exception
	ldc.i4 0xFFFFFFFE
	ldc.i4 0xAAAAAAAA
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0xAEAEAEAE
	ceq
	brfalse fail
	*/

	// 4294967295 * 0 = 0
	ldc.i4 0xFFFFFFFF
	ldc.i4 0x00000000
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0x00000000
	ceq
	brfalse fail

	// 4294967295 * 1 = 4294967295
	ldc.i4 0xFFFFFFFF
	ldc.i4 0x00000001
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0xFFFFFFFF
	ceq
	brfalse fail

	/*
	// This should throw an overflow exception
	ldc.i4 0xFFFFFFFF
	ldc.i4 0xFFFFFFFE
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0xAEAEAEAE
	ceq
	brfalse fail

	// This should throw an overflow exception
	ldc.i4 0xFFFFFFFF
	ldc.i4 0xFFFFFFFF
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0xAEAEAEAE
	ceq
	brfalse fail

	// This should throw an overflow exception
	ldc.i4 0xFFFFFFFF
	ldc.i4 0x55555555
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0xAEAEAEAE
	ceq
	brfalse fail

	// This should throw an overflow exception
	ldc.i4 0xFFFFFFFF
	ldc.i4 0xAAAAAAAA
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0xAEAEAEAE
	ceq
	brfalse fail
	*/

	// 1431655765 * 0 = 0
	ldc.i4 0x55555555
	ldc.i4 0x00000000
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0x00000000
	ceq
	brfalse fail

	// 1431655765 * 1 = 1431655765
	ldc.i4 0x55555555
	ldc.i4 0x00000001
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0x55555555
	ceq
	brfalse fail

	/*
	// This should throw an overflow exception
	ldc.i4 0x55555555
	ldc.i4 0xFFFFFFFE
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0xAEAEAEAE
	ceq
	brfalse fail

	// This should throw an overflow exception
	ldc.i4 0x55555555
	ldc.i4 0xFFFFFFFF
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0xAEAEAEAE
	ceq
	brfalse fail

	// This should throw an overflow exception
	ldc.i4 0x55555555
	ldc.i4 0x55555555
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0xAEAEAEAE
	ceq
	brfalse fail

	// This should throw an overflow exception
	ldc.i4 0x55555555
	ldc.i4 0xAAAAAAAA
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0xAEAEAEAE
	ceq
	brfalse fail
	*/

	// 2863311530 * 0 = 0
	ldc.i4 0xAAAAAAAA
	ldc.i4 0x00000000
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0x00000000
	ceq
	brfalse fail

	// 2863311530 * 1 = 2863311530
	ldc.i4 0xAAAAAAAA
	ldc.i4 0x00000001
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0xAAAAAAAA
	ceq
	brfalse fail

	/*
	// This should throw an overflow exception
	ldc.i4 0xAAAAAAAA
	ldc.i4 0xFFFFFFFE
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0xAEAEAEAE
	ceq
	brfalse fail

	// This should throw an overflow exception
	ldc.i4 0xAAAAAAAA
	ldc.i4 0xFFFFFFFF
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0xAEAEAEAE
	ceq
	brfalse fail

	// This should throw an overflow exception
	ldc.i4 0xAAAAAAAA
	ldc.i4 0x55555555
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0xAEAEAEAE
	ceq
	brfalse fail

	// This should throw an overflow exception
	ldc.i4 0xAAAAAAAA
	ldc.i4 0xAAAAAAAA
	call int32 _mul_ovf::u4(int32,int32)
	ldc.i4 0xAEAEAEAE
	ceq
	brfalse fail
	*/

pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}
}