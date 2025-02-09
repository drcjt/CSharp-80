.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.class public _clt_u {

.field public static	native uint all
.field public static	native uint none
.field public static	native uint odd
.field public static	native uint even

.method public static	void initialize() {
.maxstack	10
	ldc.i4		0xFFFFFFFF
	conv.u
	stsfld	native uint _clt_u::all
	ldc.i4		0x00000000
	conv.u
	stsfld	native uint _clt_u::none
	ldc.i4		0x55555555
	conv.u
	stsfld	native uint _clt_u::odd
	ldc.i4		0xAAAAAAAA
	conv.u
	stsfld	native uint _clt_u::even
	ret
}


.method public static int32 Main() {
.entrypoint
.maxstack 10

	call	void _clt_u::initialize()

	ldsfld	native uint _clt_u::all
	ldsfld	native uint _clt_u::all
	clt.un
	brtrue fail

	ldsfld	native uint _clt_u::all
	ldsfld	native uint _clt_u::none
	clt.un
	brtrue fail

	ldsfld	native uint _clt_u::all
	ldsfld	native uint _clt_u::odd
	clt.un
	brtrue fail

	ldsfld	native uint _clt_u::all
	ldsfld	native uint _clt_u::even
	clt.un
	brtrue fail

	ldsfld	native uint _clt_u::none
	ldsfld	native uint _clt_u::all
	clt.un
	brfalse fail

	ldsfld	native uint _clt_u::none
	ldsfld	native uint _clt_u::none
	clt.un
	brtrue fail

	ldsfld	native uint _clt_u::none
	ldsfld	native uint _clt_u::odd
	clt.un
	brfalse fail

	ldsfld	native uint _clt_u::none
	ldsfld	native uint _clt_u::even
	clt.un
	brfalse fail

	ldsfld	native uint _clt_u::odd
	ldsfld	native uint _clt_u::all
	clt.un
	brfalse fail

	ldsfld	native uint _clt_u::odd
	ldsfld	native uint _clt_u::none
	clt.un
	brtrue fail

	ldsfld	native uint _clt_u::odd
	ldsfld	native uint _clt_u::odd
	clt.un
	brtrue fail

	ldsfld	native uint _clt_u::odd
	ldsfld	native uint _clt_u::even
	clt.un
	brfalse fail

	ldsfld	native uint _clt_u::even
	ldsfld	native uint _clt_u::all
	clt.un
	brfalse fail

	ldsfld	native uint _clt_u::even
	ldsfld	native uint _clt_u::none
	clt.un
	brtrue fail

	ldsfld	native uint _clt_u::even
	ldsfld	native uint _clt_u::odd
	clt.un
	brtrue fail

	ldsfld	native uint _clt_u::even
	ldsfld	native uint _clt_u::even
	clt.un
	brtrue fail

pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}
}