.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.class public _clt_i {

.field public static	native int min
.field public static	native int _one
.field public static	native int zero
.field public static	native int one
.field public static	native int max
.field public static	native int odd
.field public static	native int even

.method public static	void initialize() {
.maxstack	10
	ldc.i4		0x00008000
	conv.i
	stsfld	native int _clt_i::min
	ldc.i4		0x0000FFFF
	conv.i
	stsfld	native int _clt_i::_one
	ldc.i4		0x00000000
	conv.i
	stsfld	native int _clt_i::zero
	ldc.i4		0x00000001
	conv.i
	stsfld	native int _clt_i::one
	ldc.i4		0x00007FFF
	conv.i
	stsfld	native int _clt_i::max
	ldc.i4		0x00005555
	conv.i
	stsfld	native int _clt_i::odd
	ldc.i4		0x0000AAAA
	conv.i
	stsfld	native int _clt_i::even
	ret
}

.method public static int32 Main() {
.entrypoint
.maxstack 10

	call	void _clt_i::initialize()

	ldsfld	native int _clt_i::min
	ldsfld	native int _clt_i::min
	clt
	brtrue fail

	ldsfld	native int _clt_i::min
	ldsfld	native int _clt_i::_one
	clt
	brfalse fail

	ldsfld	native int _clt_i::min
	ldsfld	native int _clt_i::zero
	clt
	brfalse fail

	ldsfld	native int _clt_i::min
	ldsfld	native int _clt_i::one
	clt
	brfalse fail

	ldsfld	native int _clt_i::min
	ldsfld	native int _clt_i::max
	clt
	brfalse fail

	ldsfld	native int _clt_i::min
	ldsfld	native int _clt_i::odd
	clt
	brfalse fail

	ldsfld	native int _clt_i::min
	ldsfld	native int _clt_i::even
	clt
	brfalse fail

	ldsfld	native int _clt_i::_one
	ldsfld	native int _clt_i::min
	clt
	brtrue fail

	ldsfld	native int _clt_i::_one
	ldsfld	native int _clt_i::_one
	clt
	brtrue fail

	ldsfld	native int _clt_i::_one
	ldsfld	native int _clt_i::zero
	clt
	brfalse fail

	ldsfld	native int _clt_i::_one
	ldsfld	native int _clt_i::one
	clt
	brfalse fail

	ldsfld	native int _clt_i::_one
	ldsfld	native int _clt_i::max
	clt
	brfalse fail

	ldsfld	native int _clt_i::_one
	ldsfld	native int _clt_i::odd
	clt
	brfalse fail

	ldsfld	native int _clt_i::_one
	ldsfld	native int _clt_i::even
	clt
	brtrue fail

	ldsfld	native int _clt_i::zero
	ldsfld	native int _clt_i::min
	clt
	brtrue fail

	ldsfld	native int _clt_i::zero
	ldsfld	native int _clt_i::_one
	clt
	brtrue fail

	ldsfld	native int _clt_i::zero
	ldsfld	native int _clt_i::zero
	clt
	brtrue fail

	ldsfld	native int _clt_i::zero
	ldsfld	native int _clt_i::one
	clt
	brfalse fail

	ldsfld	native int _clt_i::zero
	ldsfld	native int _clt_i::max
	clt
	brfalse fail

	ldsfld	native int _clt_i::zero
	ldsfld	native int _clt_i::odd
	clt
	brfalse fail

	ldsfld	native int _clt_i::zero
	ldsfld	native int _clt_i::even
	clt
	brtrue fail

	ldsfld	native int _clt_i::one
	ldsfld	native int _clt_i::min
	clt
	brtrue fail

	ldsfld	native int _clt_i::one
	ldsfld	native int _clt_i::_one
	clt
	brtrue fail

	ldsfld	native int _clt_i::one
	ldsfld	native int _clt_i::zero
	clt
	brtrue fail

	ldsfld	native int _clt_i::one
	ldsfld	native int _clt_i::one
	clt
	brtrue fail

	ldsfld	native int _clt_i::one
	ldsfld	native int _clt_i::max
	clt
	brfalse fail

	ldsfld	native int _clt_i::one
	ldsfld	native int _clt_i::odd
	clt
	brfalse fail

	ldsfld	native int _clt_i::one
	ldsfld	native int _clt_i::even
	clt
	brtrue fail

	ldsfld	native int _clt_i::max
	ldsfld	native int _clt_i::min
	clt
	brtrue fail

	ldsfld	native int _clt_i::max
	ldsfld	native int _clt_i::_one
	clt
	brtrue fail

	ldsfld	native int _clt_i::max
	ldsfld	native int _clt_i::zero
	clt
	brtrue fail

	ldsfld	native int _clt_i::max
	ldsfld	native int _clt_i::one
	clt
	brtrue fail

	ldsfld	native int _clt_i::max
	ldsfld	native int _clt_i::max
	clt
	brtrue fail

	ldsfld	native int _clt_i::max
	ldsfld	native int _clt_i::odd
	clt
	brtrue fail

	ldsfld	native int _clt_i::max
	ldsfld	native int _clt_i::even
	clt
	brtrue fail

	ldsfld	native int _clt_i::odd
	ldsfld	native int _clt_i::min
	clt
	brtrue fail

	ldsfld	native int _clt_i::odd
	ldsfld	native int _clt_i::_one
	clt
	brtrue fail

	ldsfld	native int _clt_i::odd
	ldsfld	native int _clt_i::zero
	clt
	brtrue fail

	ldsfld	native int _clt_i::odd
	ldsfld	native int _clt_i::one
	clt
	brtrue fail

	ldsfld	native int _clt_i::odd
	ldsfld	native int _clt_i::max
	clt
	brfalse fail

	ldsfld	native int _clt_i::odd
	ldsfld	native int _clt_i::odd
	clt
	brtrue fail

	ldsfld	native int _clt_i::odd
	ldsfld	native int _clt_i::even
	clt
	brtrue fail

	ldsfld	native int _clt_i::even
	ldsfld	native int _clt_i::min
	clt
	brtrue fail

	ldsfld	native int _clt_i::even
	ldsfld	native int _clt_i::_one
	clt
	brfalse fail

	ldsfld	native int _clt_i::even
	ldsfld	native int _clt_i::zero
	clt
	brfalse fail

	ldsfld	native int _clt_i::even
	ldsfld	native int _clt_i::one
	clt
	brfalse fail

	ldsfld	native int _clt_i::even
	ldsfld	native int _clt_i::max
	clt
	brfalse fail

	ldsfld	native int _clt_i::even
	ldsfld	native int _clt_i::odd
	clt
	brfalse fail

	ldsfld	native int _clt_i::even
	ldsfld	native int _clt_i::even
	clt
	brtrue fail

pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}
}