.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}

.class public sealed _struct extends System.ValueType
{
	.field public int32 I1
	.field public int32 I2
	.field public int32 I3
	.field public int32 I4
	.field public int32 I5
	.field public int32 I6
	.field public int32 I7
	.field public int32 I8
	.field public int32 I9
	.field public int32 I10
	.field public int32 I11
	.field public int32 I12
	.field public int32 I13
	.field public int32 I14
	.field public int32 I15
	.field public int32 I16
	.field public int32 I17
	.field public int32 I18
	.field public int32 I19
	.field public int32 I20
	.field public int32 I21
	.field public int32 I22
	.field public int32 I23
	.field public int32 I24
	.field public int32 I25
	.field public int32 I26
	.field public int32 I27
	.field public int32 I28
	.field public int32 I29
	.field public int32 I30
	.field public int32 I31
	.field public int32 I32
}

.class ldloc_stloc {
.method public static int32 Main() {
.entrypoint
.maxstack 10
.locals (valuetype _struct, int32, int16, uint8)


	ldc.i4	0x7FFFFFFF
	stloc	1
	ldc.i4	0x7FFFFFFF
	ldloc	1
	ceq
	brfalse fail

	ldc.i4  0x1
	stloc   2
	ldc.i4  0x1
	ldloc   2
	ceq
	brfalse fail

	ldc.i4  0x1
	stloc   3
	ldc.i4  0x1
	ldloc   3
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