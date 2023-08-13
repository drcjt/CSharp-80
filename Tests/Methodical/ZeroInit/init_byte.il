.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}

.assembly TestAssembly
{
	.ver 0:0:0:0
}

.class public Test_Init_Byte
{

.method private static int8 Ident(int8)
{
.maxstack 1
	ldarga.s 0
	ldind.i1
	ret
}

.method private static int32 IdentAddr(int8&)
{
.maxstack 1
	ldarga.s 0
	ldind.i
	ldind.i1
	ret
}


// Test 1
// 2 Local Variables, one is used as return
// Expect only v1 to be initialized to 0
.method private static int8 Test1() {
.maxstack 1
.locals init (int8 v0, int8 v1)
	ldloc.1
	ret
}

.method public static int32 Main() {
.entrypoint
.maxstack  5
.locals ()

	call int8 Test_Init_Byte::Test1()
	brfalse t2

	ldc.i4.1
	br FAIL

t2:

PASS:
	ldc.i4 0x0000
	ret
FAIL:
	ret
}
}