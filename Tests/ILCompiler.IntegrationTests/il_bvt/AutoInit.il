﻿.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
	.ver 0:0:0:0
}
.class public auto_init
{
.method public static int32 Main() {
.zeroinit
.entrypoint
.locals (int32,int32,int32,int32,int32,int32,int32,int32,int32,int32,int32,int32,int32,int32,int32,int32)
.maxstack  2

	ldc.i4 0x1
	stloc 0x1
	ldc.i4 0x3
	stloc 0x3
	ldc.i4 0x5
	stloc 0x5
	ldc.i4 0x7
	stloc 0x7
	ldc.i4 0x9
	stloc 0x9
	ldc.i4 0xB
	stloc 0xB
	ldc.i4 0xD
	stloc 0xD
	ldc.i4 0xF
	stloc 0xF

	ldloc 0x0
	ldloc 0x1
	add
	ldloc 0x2
	add
	ldloc 0x3
	add
	ldloc 0x4
	add
	ldloc 0x5
	add
	ldloc 0x6
	add
	ldloc 0x7
	add
	ldloc 0x8
	add
	ldloc 0x9
	add
	ldloc 0xA
	add
	ldloc 0xB
	add
	ldloc 0xC
	add
	ldloc 0xD
	add
	ldloc 0xE
	add
	ldloc 0xF
	add

	ldc.i4	0x40
	ceq
	brfalse FAIL

PASS:
	ldc.i4 0x0000
	ret
FAIL:
	ldc.i4 0x0001
	ret
}
}