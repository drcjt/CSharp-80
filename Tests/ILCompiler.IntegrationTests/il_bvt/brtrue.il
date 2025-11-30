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
	ldc.i4 0x1
    stloc   0
    ldloc   0
	brtrue next0
	br fail

next0:
	ldc.i4 0x2
    stloc   0
    ldloc   0
    brtrue next1
	br fail

next1:
	ldnull
    stloc   0
    ldloc   0
	brtrue fail

	ldc.i4 0x1
    stloc   0
    ldloc   0
	conv.i
	brtrue next2
	br fail

next2:
    // -1
    ldc.i4 0xFFFFFFFF
    stloc   0
    ldloc   0
    brtrue next3
    br fail

next3:
    // 2147483647 
    ldc.i4 0x7FFFFFFF
    stloc   0
    ldloc   0
    brtrue next4
    br fail

next4:
    // -2147483648
    ldc.i4 0x80000000
    stloc   0
    ldloc   0
    brtrue next5
    br fail

next5:
    // 1431655765
    ldc.i4 0x55555555
    stloc   0
    ldloc   0
    brtrue next6
    br fail

next6:
    // -1431655766
    ldc.i4 0xAAAAAAAA
    stloc   0
    ldloc   0
    brtrue next7
    br fail

next7:
    // 0
    ldc.i4 0x00000000
    stloc   0
    ldloc   0
    brtrue fail

pass:
	ldc.i4 0x0000
	ret

fail:
	ldc.i4 0x0001
	ret
}
