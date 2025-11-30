.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.method public static int32 Main() {
.entrypoint
.locals (int32)
	ldc.i4 0x0
    stloc   0
    ldloc   0
	brfalse next1
	br fail

next1:
	ldnull
    stloc   0
    ldloc   0
	brfalse next2
	br fail

next2:
	ldc.i4.0
    stloc   0
    ldloc   0
	conv.i
	brfalse next3
    br fail

next3:
    // -1
    ldc.i4 0xFFFFFFFF
    stloc   0
    ldloc   0
    brfalse fail

    // 2147483647 
    ldc.i4 0x7FFFFFFF
    stloc   0
    ldloc   0
    brfalse fail

    // -2147483648
    ldc.i4 0x80000000
    stloc   0
    ldloc   0
    brfalse fail

    // 1431655765
    ldc.i4 0x55555555
    stloc   0
    ldloc   0
    brfalse fail

    // -1431655766
    ldc.i4 0xAAAAAAAA
    stloc   0
    ldloc   0
    brfalse fail

    // 0
    ldc.i4 0x00000000
    stloc   0
    ldloc   0
    brfalse pass

fail:
	ldc.i4 0x0001
	ret
pass:
	ldc.i4 0x0000
	ret
}
