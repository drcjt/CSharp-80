.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.class public _clt_un {

.field public static	int32 all
.field public static	int32 none
.field public static	int32 odd
.field public static	int32 even

.method public static	void initialize() {
.maxstack	10
	ldc.i4		0xFFFFFFFF
	stsfld	int32 _clt_un::all
	ldc.i4		0x00000000
	stsfld	int32 _clt_un::none
	ldc.i4		0x55555555
	stsfld	int32 _clt_un::odd
	ldc.i4		0xAAAAAAAA
	stsfld	int32 _clt_un::even
	ret
}


.method public static int32 Main() {
.entrypoint
.maxstack 10

	call	void _clt_un::initialize()

	ldsfld	int32 _clt_un::all
	ldsfld	int32 _clt_un::all
	clt.un
	brtrue fail

	ldsfld	int32 _clt_un::all
	ldsfld	int32 _clt_un::none
	clt.un
	brtrue fail

	ldsfld	int32 _clt_un::all
	ldsfld	int32 _clt_un::odd
	clt.un
	brtrue fail

	ldsfld	int32 _clt_un::all
	ldsfld	int32 _clt_un::even
	clt.un
	brtrue fail

	ldsfld	int32 _clt_un::none
	ldsfld	int32 _clt_un::all
	clt.un
	brfalse fail

	ldsfld	int32 _clt_un::none
	ldsfld	int32 _clt_un::none
	clt.un
	brtrue fail

	ldsfld	int32 _clt_un::none
	ldsfld	int32 _clt_un::odd
	clt.un
	brfalse fail

	ldsfld	int32 _clt_un::none
	ldsfld	int32 _clt_un::even
	clt.un
	brfalse fail

	ldsfld	int32 _clt_un::odd
	ldsfld	int32 _clt_un::all
	clt.un
	brfalse fail

	ldsfld	int32 _clt_un::odd
	ldsfld	int32 _clt_un::none
	clt.un
	brtrue fail

	ldsfld	int32 _clt_un::odd
	ldsfld	int32 _clt_un::odd
	clt.un
	brtrue fail

	ldsfld	int32 _clt_un::odd
	ldsfld	int32 _clt_un::even
	clt.un
	brfalse fail

	ldsfld	int32 _clt_un::even
	ldsfld	int32 _clt_un::all
	clt.un
	brfalse fail

	ldsfld	int32 _clt_un::even
	ldsfld	int32 _clt_un::none
	clt.un
	brtrue fail

	ldsfld	int32 _clt_un::even
	ldsfld	int32 _clt_un::odd
	clt.un
	brtrue fail

	ldsfld	int32 _clt_un::even
	ldsfld	int32 _clt_un::even
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