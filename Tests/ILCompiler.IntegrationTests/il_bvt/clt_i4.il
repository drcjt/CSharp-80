.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.class public _clt {

.field public static	int32 min
.field public static	int32 _one
.field public static	int32 zero
.field public static	int32 one
.field public static	int32 max
.field public static	int32 odd
.field public static	int32 even

.method public static	void initialize() {
.maxstack	10
	ldc.i4		0x80000000
	stsfld	int32 _clt::min
	ldc.i4		0xFFFFFFFF
	stsfld	int32 _clt::_one
	ldc.i4		0x00000000
	stsfld	int32 _clt::zero
	ldc.i4		0x00000001
	stsfld	int32 _clt::one
	ldc.i4		0x7FFFFFFF
	stsfld	int32 _clt::max
	ldc.i4		0x55555555
	stsfld	int32 _clt::odd
	ldc.i4		0xAAAAAAAA
	stsfld	int32 _clt::even
	ret
}


.method public static int32 Main() {
.entrypoint
.maxstack 10

	call	void _clt::initialize()

	ldsfld	int32 _clt::min
	ldsfld	int32 _clt::min
	clt
	brtrue fail

	ldsfld	int32 _clt::min
	ldsfld	int32 _clt::_one
	clt
	brfalse fail

	ldsfld	int32 _clt::min
	ldsfld	int32 _clt::zero
	clt
	brfalse fail

	ldsfld	int32 _clt::min
	ldsfld	int32 _clt::one
	clt
	brfalse fail

	ldsfld	int32 _clt::min
	ldsfld	int32 _clt::max
	clt
	brfalse fail

	ldsfld	int32 _clt::min
	ldsfld	int32 _clt::odd
	clt
	brfalse fail

	ldsfld	int32 _clt::min
	ldsfld	int32 _clt::even
	clt
	brfalse fail

	ldsfld	int32 _clt::_one
	ldsfld	int32 _clt::min
	clt
	brtrue fail

	ldsfld	int32 _clt::_one
	ldsfld	int32 _clt::_one
	clt
	brtrue fail

	ldsfld	int32 _clt::_one
	ldsfld	int32 _clt::zero
	clt
	brfalse fail

	ldsfld	int32 _clt::_one
	ldsfld	int32 _clt::one
	clt
	brfalse fail

	ldsfld	int32 _clt::_one
	ldsfld	int32 _clt::max
	clt
	brfalse fail

	ldsfld	int32 _clt::_one
	ldsfld	int32 _clt::odd
	clt
	brfalse fail

	ldsfld	int32 _clt::_one
	ldsfld	int32 _clt::even
	clt
	brtrue fail

	ldsfld	int32 _clt::zero
	ldsfld	int32 _clt::min
	clt
	brtrue fail

	ldsfld	int32 _clt::zero
	ldsfld	int32 _clt::_one
	clt
	brtrue fail

	ldsfld	int32 _clt::zero
	ldsfld	int32 _clt::zero
	clt
	brtrue fail

	ldsfld	int32 _clt::zero
	ldsfld	int32 _clt::one
	clt
	brfalse fail

	ldsfld	int32 _clt::zero
	ldsfld	int32 _clt::max
	clt
	brfalse fail

	ldsfld	int32 _clt::zero
	ldsfld	int32 _clt::odd
	clt
	brfalse fail

	ldsfld	int32 _clt::zero
	ldsfld	int32 _clt::even
	clt
	brtrue fail

	ldsfld	int32 _clt::one
	ldsfld	int32 _clt::min
	clt
	brtrue fail

	ldsfld	int32 _clt::one
	ldsfld	int32 _clt::_one
	clt
	brtrue fail

	ldsfld	int32 _clt::one
	ldsfld	int32 _clt::zero
	clt
	brtrue fail

	ldsfld	int32 _clt::one
	ldsfld	int32 _clt::one
	clt
	brtrue fail

	ldsfld	int32 _clt::one
	ldsfld	int32 _clt::max
	clt
	brfalse fail

	ldsfld	int32 _clt::one
	ldsfld	int32 _clt::odd
	clt
	brfalse fail

	ldsfld	int32 _clt::one
	ldsfld	int32 _clt::even
	clt
	brtrue fail

	ldsfld	int32 _clt::max
	ldsfld	int32 _clt::min
	clt
	brtrue fail

	ldsfld	int32 _clt::max
	ldsfld	int32 _clt::_one
	clt
	brtrue fail

	ldsfld	int32 _clt::max
	ldsfld	int32 _clt::zero
	clt
	brtrue fail

	ldsfld	int32 _clt::max
	ldsfld	int32 _clt::one
	clt
	brtrue fail

	ldsfld	int32 _clt::max
	ldsfld	int32 _clt::max
	clt
	brtrue fail

	ldsfld	int32 _clt::max
	ldsfld	int32 _clt::odd
	clt
	brtrue fail

	ldsfld	int32 _clt::max
	ldsfld	int32 _clt::even
	clt
	brtrue fail

	ldsfld	int32 _clt::odd
	ldsfld	int32 _clt::min
	clt
	brtrue fail

	ldsfld	int32 _clt::odd
	ldsfld	int32 _clt::_one
	clt
	brtrue fail

	ldsfld	int32 _clt::odd
	ldsfld	int32 _clt::zero
	clt
	brtrue fail

	ldsfld	int32 _clt::odd
	ldsfld	int32 _clt::one
	clt
	brtrue fail

	ldsfld	int32 _clt::odd
	ldsfld	int32 _clt::max
	clt
	brfalse fail

	ldsfld	int32 _clt::odd
	ldsfld	int32 _clt::odd
	clt
	brtrue fail

	ldsfld	int32 _clt::odd
	ldsfld	int32 _clt::even
	clt
	brtrue fail

	ldsfld	int32 _clt::even
	ldsfld	int32 _clt::min
	clt
	brtrue fail

	ldsfld	int32 _clt::even
	ldsfld	int32 _clt::_one
	clt
	brfalse fail

	ldsfld	int32 _clt::even
	ldsfld	int32 _clt::zero
	clt
	brfalse fail

	ldsfld	int32 _clt::even
	ldsfld	int32 _clt::one
	clt
	brfalse fail

	ldsfld	int32 _clt::even
	ldsfld	int32 _clt::max
	clt
	brfalse fail

	ldsfld	int32 _clt::even
	ldsfld	int32 _clt::odd
	clt
	brfalse fail

	ldsfld	int32 _clt::even
	ldsfld	int32 _clt::even
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