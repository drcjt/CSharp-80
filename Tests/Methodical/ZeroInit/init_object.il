.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}

.assembly TestAssembly
{
	.ver 0:0:0:0
}

.class public TestClass {

.field private int32 i1;

.method public void .ctor(){
.maxstack 10
	ldarg.0
	call instance void [mscorlib]System.Object::.ctor()
	ret
}

.method public int32 TestMethod() {
.maxstack 10

	ldarg.0
	ldfld int32 TestClass::i1
	ret
}

}

.class public Test_Init_Object
{

.method public static int32 Main() {
.entrypoint
.maxstack  5
.locals ()

	newobj instance void TestClass::.ctor()
	call instance int32 TestClass::TestMethod()
	ldc.i4 0x0
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