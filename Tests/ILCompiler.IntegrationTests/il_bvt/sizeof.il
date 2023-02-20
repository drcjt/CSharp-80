.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}

.class public sequential ansi sealed beforefieldinit Struct0 extends System.ValueType
{
	.field private bool b0;
	.field private int32 i1;
}

.class public SizeOf
{

.method public void SizeOf()
{
.maxstack 0
	ret
}

.method public static int32 Main() {
.entrypoint
.maxstack 20
.locals (class SizeOf)

	sizeof int8
	ldc.i4 0x1
	bne.un fail

	sizeof int16
	ldc.i4 0x2
	bne.un fail

	sizeof int32
	ldc.i4 0x4
	bne.un fail

	sizeof unsigned int8
	ldc.i4 0x1
	bne.un fail

	sizeof unsigned int16
	ldc.i4 0x2
	bne.un fail

	sizeof unsigned int32
	ldc.i4 0x4
	bne.un fail

	sizeof SizeOf
	ldc.i4 0x2
	bne.un fail

	sizeof Struct0
	ldc.i4 0x8
	bne.un fail

pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}
}