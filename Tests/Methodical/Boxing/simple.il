.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}

.assembly TestAssembly
{
	.ver 0:0:0:0
}

.class public Simple_BoxUnbox
{

.method public static int32 Main()
{
.entrypoint
.maxstack 4
	ldc.i4.s 100

	box [mscorlib]System.Int32
	unbox [mscorlib]System.Int32
	ldind.i4

	conv.i1
	box [mscorlib]System.SByte
	unbox [mscorlib]System.SByte
	ldind.i1

	box [mscorlib]System.SByte
	unbox [mscorlib]System.SByte
	ldind.i1

	conv.u1
	box [mscorlib]System.Byte
	unbox [mscorlib]System.Byte
	ldind.u1

	conv.i
	box [mscorlib]System.IntPtr
	unbox [mscorlib]System.IntPtr
	ldobj [mscorlib]System.IntPtr

	conv.u4
	box [mscorlib]System.UInt32
	unbox [mscorlib]System.UInt32
	ldind.u4

	conv.u
	box [mscorlib]System.UIntPtr
	unbox [mscorlib]System.UIntPtr
	ldobj [mscorlib]System.UIntPtr

	conv.i4

	ldc.i4 100
	beq.s pass

fail:
	ldc.i4 0x0001
	ret

pass:
	ldc.i4 0x0000
	ret
}
}