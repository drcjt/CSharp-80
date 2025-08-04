.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.class public _not
{
.method public static int32 not_i4(int32,int32) {
.maxstack 2
	ldarg 0
	not
    ldarg 1
    ceq
	ret
}

.method public static int32 Main() {
.entrypoint
.maxstack  10

    // not(-2147483648) = 2147483647
	ldc.i4 0x80000000
	ldc.i4 0x7FFFFFFF
	call int32 _not::not_i4(int32,int32)
    brfalse fail

    // not(-2147483647) = 2147483646
	ldc.i4 0x80000001
	ldc.i4 0x7FFFFFFE
	call int32 _not::not_i4(int32,int32)
    brfalse fail

    // not(-1) = 0
	ldc.i4 0xFFFFFFFF
	ldc.i4 0x00000000
	call int32 _not::not_i4(int32,int32)
    brfalse fail

    // not(0) = -1
	ldc.i4 0x00000000
	ldc.i4 0xFFFFFFFF
	call int32 _not::not_i4(int32,int32)
    brfalse fail

    // not(1) = -2
	ldc.i4 0x00000001
	ldc.i4 0xFFFFFFFE
	call int32 _not::not_i4(int32,int32)
    brfalse fail

    // not(2147483646) = -2147483647
	ldc.i4 0x7FFFFFFE
	ldc.i4 0x80000001
	call int32 _not::not_i4(int32,int32)
    brfalse fail

    // not(2147483647) = -2147483648
	ldc.i4 0x7FFFFFFF
   	ldc.i4 0x80000000
	call int32 _not::not_i4(int32,int32)
    brfalse fail

    // not(1431655765) = -1431655765
	ldc.i4 0x55555555
	ldc.i4 0xAAAAAAAA
	call int32 _not::not_i4(int32,int32)
    brfalse fail

    // not(-1431655765) = 1431655765
	ldc.i4 0xAAAAAAAA
	ldc.i4 0x55555555
	call int32 _not::not_i4(int32,int32)
    brfalse fail

pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}
}
