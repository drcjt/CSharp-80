.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}

.class public Super {
.method public void .ctor() {
.maxstack 10
	ldarg 0
	call instance void [mscorlib]System.Object::.ctor()
	ret
}
}

.class public Sub extends Super {
.method public void .ctor() {
.maxstack 10
	ldarg 0
	call instance void Super::.ctor()
	ret
}
}

.class public _isinst {

.method public static int32 Main() {
.entrypoint
.maxstack 10
.locals(int32)

	newobj instance void Sub::.ctor()
	isinst Sub
	brfalse fail

	newobj instance void Sub::.ctor()
	isinst Super
	brfalse fail

	newobj instance void Super::.ctor()
	isinst Sub
	brtrue fail
pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}
}