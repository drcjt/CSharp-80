.assembly extern mscorlib
{
    .ver 4:0:0:0
    .publickeytoken = (B7 7A 5C 56 19 34 E0 89)
}
.assembly callnonvirt
{
}
.class public TestClass {

.method public void .ctor(){
.maxstack 10
	ldarg.0
	call instance void [mscorlib]System.Object::.ctor()
	ret
}

.method public int32 TestMethod(int32) {
.maxstack 10

	ldarg.1
	ldc.i4 0x2
	mul
	ret
}

}

.class public callnonvirt
{
.method public static int32 Main() {
.entrypoint
.maxstack  10

	newobj instance void TestClass::.ctor()
	ldc.i4 0x2
	call instance int32 TestClass::TestMethod(int32)
	ldc.i4 0x4
	ceq
	brfalse FAIL
PASS:
	ldc.i4 0x0000
	ret
FAIL:
	ldc.i4 0x0001
	ret
}
}