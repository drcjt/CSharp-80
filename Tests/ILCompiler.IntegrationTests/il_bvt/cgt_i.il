.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.class public _cgt_i {

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
	stsfld	native int _cgt_i::min
	ldc.i4		0x0000FFFF
	conv.i
	stsfld	native int _cgt_i::_one
	ldc.i4		0x00000000
	conv.i
	stsfld	native int _cgt_i::zero
	ldc.i4		0x00000001
	conv.i
	stsfld	native int _cgt_i::one
	ldc.i4		0x00007FFF
	conv.i
	stsfld	native int _cgt_i::max
	ldc.i4		0x00005555
	conv.i
	stsfld	native int _cgt_i::odd
	ldc.i4		0x0000AAAA
	conv.i
	stsfld	native int _cgt_i::even
	ret
}


.method public static int32 Main() {
.entrypoint
.maxstack 10

	call	void _cgt_i::initialize()

	ldsfld	native int _cgt_i::min
	ldsfld	native int _cgt_i::min
	cgt
	brtrue fail

	ldsfld	native int _cgt_i::min
	ldsfld	native int _cgt_i::_one
	cgt
	brtrue fail

	ldsfld	native int _cgt_i::min
	ldsfld	native int _cgt_i::zero
	cgt
	brtrue fail

	ldsfld	native int _cgt_i::min
	ldsfld	native int _cgt_i::one
	cgt
	brtrue fail

	ldsfld	native int _cgt_i::min
	ldsfld	native int _cgt_i::max
	cgt
	brtrue fail

	ldsfld	native int _cgt_i::min
	ldsfld	native int _cgt_i::odd
	cgt
	brtrue fail

	ldsfld	native int _cgt_i::min
	ldsfld	native int _cgt_i::even
	cgt
	brtrue fail

	ldsfld	native int _cgt_i::_one
	ldsfld	native int _cgt_i::min
	cgt
	brfalse fail

	ldsfld	native int _cgt_i::_one
	ldsfld	native int _cgt_i::_one
	cgt
	brtrue fail

	ldsfld	native int _cgt_i::_one
	ldsfld	native int _cgt_i::zero
	cgt
	brtrue fail

	ldsfld	native int _cgt_i::_one
	ldsfld	native int _cgt_i::one
	cgt
	brtrue fail

	ldsfld	native int _cgt_i::_one
	ldsfld	native int _cgt_i::max
	cgt
	brtrue fail

	ldsfld	native int _cgt_i::_one
	ldsfld	native int _cgt_i::odd
	cgt
	brtrue fail

	ldsfld	native int _cgt_i::_one
	ldsfld	native int _cgt_i::even
	cgt
	brfalse fail

	ldsfld	native int _cgt_i::zero
	ldsfld	native int _cgt_i::min
	cgt
	brfalse fail

	ldsfld	native int _cgt_i::zero
	ldsfld	native int _cgt_i::_one
	cgt
	brfalse fail

	ldsfld	native int _cgt_i::zero
	ldsfld	native int _cgt_i::zero
	cgt
	brtrue fail

	ldsfld	native int _cgt_i::zero
	ldsfld	native int _cgt_i::one
	cgt
	brtrue fail

	ldsfld	native int _cgt_i::zero
	ldsfld	native int _cgt_i::max
	cgt
	brtrue fail

	ldsfld	native int _cgt_i::zero
	ldsfld	native int _cgt_i::odd
	cgt
	brtrue fail

	ldsfld	native int _cgt_i::zero
	ldsfld	native int _cgt_i::even
	cgt
	brfalse fail

	ldsfld	native int _cgt_i::one
	ldsfld	native int _cgt_i::min
	cgt
	brfalse fail

	ldsfld	native int _cgt_i::one
	ldsfld	native int _cgt_i::_one
	cgt
	brfalse fail

	ldsfld	native int _cgt_i::one
	ldsfld	native int _cgt_i::zero
	cgt
	brfalse fail

	ldsfld	native int _cgt_i::one
	ldsfld	native int _cgt_i::one
	cgt
	brtrue fail

	ldsfld	native int _cgt_i::one
	ldsfld	native int _cgt_i::max
	cgt
	brtrue fail

	ldsfld	native int _cgt_i::one
	ldsfld	native int _cgt_i::odd
	cgt
	brtrue fail

	ldsfld	native int _cgt_i::one
	ldsfld	native int _cgt_i::even
	cgt
	brfalse fail

	//

	ldsfld	native int _cgt_i::max
	ldsfld	native int _cgt_i::min
	cgt
	brfalse fail

	ldsfld	native int _cgt_i::max
	ldsfld	native int _cgt_i::_one
	cgt
	brfalse fail

	ldsfld	native int _cgt_i::max
	ldsfld	native int _cgt_i::zero
	cgt
	brfalse fail

	ldsfld	native int _cgt_i::max
	ldsfld	native int _cgt_i::one
	cgt
	brfalse fail

	ldsfld	native int _cgt_i::max
	ldsfld	native int _cgt_i::max
	cgt
	brtrue fail

	ldsfld	native int _cgt_i::max
	ldsfld	native int _cgt_i::odd
	cgt
	brfalse fail

	ldsfld	native int _cgt_i::max
	ldsfld	native int _cgt_i::even
	cgt
	brfalse fail

	ldsfld	native int _cgt_i::odd
	ldsfld	native int _cgt_i::min
	cgt
	brfalse fail

	ldsfld	native int _cgt_i::odd
	ldsfld	native int _cgt_i::_one
	cgt
	brfalse fail

	ldsfld	native int _cgt_i::odd
	ldsfld	native int _cgt_i::zero
	cgt
	brfalse fail

	ldsfld	native int _cgt_i::odd
	ldsfld	native int _cgt_i::one
	cgt
	brfalse fail

	ldsfld	native int _cgt_i::odd
	ldsfld	native int _cgt_i::max
	cgt
	brtrue fail

	ldsfld	native int _cgt_i::odd
	ldsfld	native int _cgt_i::odd
	cgt
	brtrue fail

	ldsfld	native int _cgt_i::odd
	ldsfld	native int _cgt_i::even
	cgt
	brfalse fail

	ldsfld	native int _cgt_i::even
	ldsfld	native int _cgt_i::min
	cgt
	brfalse fail

	ldsfld	native int _cgt_i::even
	ldsfld	native int _cgt_i::_one
	cgt
	brtrue fail

	ldsfld	native int _cgt_i::even
	ldsfld	native int _cgt_i::zero
	cgt
	brtrue fail

	ldsfld	native int _cgt_i::even
	ldsfld	native int _cgt_i::one
	cgt
	brtrue fail

	ldsfld	native int _cgt_i::even
	ldsfld	native int _cgt_i::max
	cgt
	brtrue fail

	ldsfld	native int _cgt_i::even
	ldsfld	native int _cgt_i::odd
	cgt
	brtrue fail

	ldsfld	native int _cgt_i::even
	ldsfld	native int _cgt_i::even
	cgt
	brtrue fail

pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}
}