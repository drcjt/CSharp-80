.assembly extern mscorlib
{
    .ver 4:0:0:0
    .publickeytoken = (B7 7A 5C 56 19 34 E0 89)
}

.class public Test {
.method public void .ctor() {
.maxstack 10
	ldarg 0
	call instance void [mscorlib]System.Object::.ctor()
	ret
}
}

.class _newobj {
.method public static int32 Main() {
.entrypoint
.maxstack 10

	newobj instance void Test::.ctor()
	ldc.i4.0	// TODO: change to ldnull once it's implemented
	conv.i
	ceq
	brtrue fail	// fail if allocated objref is 0

pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}
}