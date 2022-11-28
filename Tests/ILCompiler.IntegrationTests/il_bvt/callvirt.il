.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.class public TestClass
{
.field public int32 X

.method public void .ctor(int32) 
{
.maxstack 10
	ldarg 0
	call instance void [mscorlib]System.Object::.ctor()
	ldarg 0
	ldarg 1
	stfld int32 TestClass::X
	ret
}

.method public int32 TestMethod(int32)
{
.maxstack 10
	ldarg 1
	ldarg 0
	ldfld int32 TestClass::X
	mul
	ret
}

}

.class public _callvirt
{

.method public static int32 Main() 
{
.entrypoint
.maxstack 10
	
	ldc.i4 0x3
	newobj instance void TestClass::.ctor(int32)
	ldc.i4 0x2
	callvirt instance int32 TestClass::TestMethod(int32)
	ldc.i4 0x6
	ceq
	brfalse fail

pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}
}