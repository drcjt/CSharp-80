.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}

.class public Test {
.method public void .ctor() {
.maxstack 10
	ldarg 0
	call instance void [mscorlib]System.Object::.ctor()
	ret
}
}

.class public _cgt_ref {

.method public static int32 Main() {
.entrypoint
.maxstack 10

	newobj instance void Test::.ctor()
	ldnull
	cgt.un
	brfalse fail

pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}
}