.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}

.assembly TestAssembly
{
	.ver 0:0:0:0
}

.class public Test_Init_Int32
{
.method private static int32 Test1() {
.maxstack 1
.locals init (int32 v0, int32 v1)
	ldloc.1
	ret
}

.method public static int32 Main() {
.entrypoint
.maxstack  5
.locals ()

	call int32 Test_Init_Int32::Test1()
	brfalse PASS	
	br FAIL

PASS:
	ldc.i4 0x0000
	ret
FAIL:
	ldc.i4 0x0001
	ret
}
}