.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.class public bge_un
{
.field public static	int32 all
.field public static	int32 none
.field public static	int32 odd
.field public static	int32 even

.method public static	void initialize() {
.maxstack	10
	ldc.i4		0xFFFFFFFF
	stsfld	int32 bge_un::all
	ldc.i4		0x00000000
	stsfld	int32 bge_un::none
	ldc.i4		0x55555555
	stsfld	int32 bge_un::odd
	ldc.i4		0xAAAAAAAA
	stsfld	int32 bge_un::even
	ret
}

.method public static int32 Main() {
.entrypoint
.maxstack	10

	call void bge_un::initialize()

	ldsfld int32 bge_un::all
	ldsfld int32 bge_un::all
	bge.un next0
	br fail

next0:
	ldsfld int32 bge_un::all
	ldsfld int32 bge_un::none
	bge.un next1
	br fail
next1:
	ldsfld int32 bge_un::all
	ldsfld int32 bge_un::odd
	bge.un next2
	br fail
next2:
	ldsfld int32 bge_un::all
	ldsfld int32 bge_un::even
	bge.un next3
	br fail
next3:
	ldsfld int32 bge_un::none
	ldsfld int32 bge_un::all
	bge.un fail

	ldsfld int32 bge_un::none
	ldsfld int32 bge_un::none
	bge.un next4
	br fail

next4:
	ldsfld int32 bge_un::none
	ldsfld int32 bge_un::odd
	bge.un fail

	ldsfld int32 bge_un::none
	ldsfld int32 bge_un::even
	bge.un fail

	ldsfld int32 bge_un::odd
	ldsfld int32 bge_un::all
	bge.un fail

	ldsfld int32 bge_un::odd
	ldsfld int32 bge_un::none
	bge.un next5
	br fail

next5:
	ldsfld int32 bge_un::odd
	ldsfld int32 bge_un::odd
	bge.un next6
	br fail

next6:
	ldsfld int32 bge_un::odd
	ldsfld int32 bge_un::even
	bge.un fail

	ldsfld int32 bge_un::even
	ldsfld int32 bge_un::all
	bge.un fail

	ldsfld int32 bge_un::even
	ldsfld int32 bge_un::none
	bge.un next7
	br fail

next7:
	ldsfld int32 bge_un::even
	ldsfld int32 bge_un::odd
	bge.un next8
	br fail

next8:
	ldsfld int32 bge_un::even
	ldsfld int32 bge_un::even
	bge.un pass
	br fail

pass:
	ldc.i4 0x0000
	br end
fail:
	ldc.i4 0x0001
	br end
end:
	ret
}
} // end of class bge_un_i4