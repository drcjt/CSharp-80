.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}

.assembly TestAssembly
{
	.ver 0:0:0:0
}
.class public add_i
{

.field public static int32 PTR

.method public static int32 Main() {
.entrypoint
.maxstack  20
.locals ()

	ldc.i4 0x000000FF
	conv.i
	ldsflda int32 add_i::PTR
	add
	conv.i4

	ldc.i4 0x000000FF
	ldsflda int32 add_i::PTR
	conv.i4
	add

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