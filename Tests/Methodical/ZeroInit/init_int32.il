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
.field private int32 _ret;
.field private static class Test_Init_Int32 global;

.method private static void .cctor()
{
.maxstack 1
	newobj instance void Test_Init_Int32::.ctor()
	stsfld class Test_Init_Int32 Test_Init_Int32::global
	ret
}

.method public instance void .ctor()
{
.maxstack 2
	ldarg.0
	ldc.i4.0
	stfld int32 Test_Init_Int32::_ret
	ldarg.0
	call instance void [mscorlib]System.Object::.ctor()
	ret
}

.method private static int32 Ident(int32)
{
.maxstack 1
	ldarga.s 0
	ldind.i4
	ret
}

.method private static int32 IdentAddr(int32&)
{
.maxstack 1
	ldarga.s 0
	ldind.i
	ldind.i4
	ret
}

// Test Summary
//

// Tests	Description
// 1		return v0
// 2		v2 = (((v0 >> 1) & 1) - (v1 << 2)) + 11; return v2
// 3		v1 = Ident(v0); return v1
// 4		v1 = IdentAddr(&v0); return v1

// Use method parameter which is pointer to local variable in main to store result
// 5		ret = Ident(v0)
// 6		ret = IdentAddr(&v0)

// Static globals
// 7		global::m_ret = v0
// 8		global::m_ret = (((v0 >> 1) & 1) - (v1 << 2)) + 11
// 9		global::m_ret = Ident(v0)
// 10		global::m_retr = IdentAddr(&v0)


// Test 1
// 2 Local Variables, one is used as return
// Expect only v1 to be initialized to 0
.method private static int32 Test1() {
.maxstack 1
.locals init (int32 v0, int32 v1)
	ldloc.1
	ret
}

// Test 2
// Zero init v0 and v1 only.
// v2 = (((v0 >> 1) & 1) - (v1 << 2)) + 11
// return = v2
.method private static int32 Test2() {
.maxstack 3
.locals init (int32 v0, int32 v1, int32 v2)
	ldloc.0
	ldc.i4.1
	shr
	ldc.i4.1
	and
	ldloc.1
	ldc.i4.2
	shl
	sub
	ldc.i4.s 11
	add
	stloc.2
	ldloc.2
	ret
}

// Test 3
// v0 will be zero initialized
.method private static int32 Test3() {
.maxstack 1
.locals init (int32 v0, int32 v1)
	ldloc.0
	call int32 Test_Init_Int32::Ident(int32)
	stloc.1
	ldloc.1
	ret
}

// Test 4
// v0 will be zero initialized
.method private static int32 Test4() {
.maxstack 1
.locals init (int32 v0, int32 v1)
	ldloca.s 0
	call int32 Test_Init_Int32::IdentAddr(int32&)
	stloc.1
	ldloc.1
	ret
}

// Test 5
// v0 will be zero initialized
.method private static void Test5(int32&) {
.maxstack 2
.locals init (int32 v0, int32 v1)
	ldarg.0
	ldloc.0
	call int32 Test_Init_Int32::Ident(int32)
	stind.i4
	ret
}

// Test 6
// v0 will be zero initialized
.method private static void Test6(int32&) {
.maxstack 2
.locals init (int32 v0, int32 v1)
	ldarg.0
	ldloca.s v0
	call int32 Test_Init_Int32::IdentAddr(int32&)
	stind.i4
	ret
}

// Test 7
// v0 wil be zero initialized
.method private static void Test7()
{
.maxstack 2
.locals init (int32 v0)
	ldsfld class Test_Init_Int32 Test_Init_Int32::global
	ldloc.0
	stfld int32 Test_Init_Int32::_ret
	ret
}

// Test 8
// v0 and v1 will be zero initialized
.method private static void Test8()
{
.maxstack 4
.locals init (int32 v0, int32 v1)
	ldsfld class Test_Init_Int32 Test_Init_Int32::global
	ldloc.0
	ldc.i4.1
	shr
	ldc.i4.1
	and
	ldloc.1
	ldc.i4.2
	shl
	sub
	ldc.i4.s 11
	add
	stfld int32 Test_Init_Int32::_ret
	ret
}

// Test 9
// v0 will be zero initialized
.method private static void Test9()
{
.maxstack 2
.locals init (int32 v0)
	ldsfld class Test_Init_Int32 Test_Init_Int32::global
	ldloc.0
	call int32 Test_Init_Int32::Ident(int32)
	stfld int32 Test_Init_Int32::_ret
	ret
}

// Test 10
// v0 will be zero initialized
.method private static void Test10()
{
.maxstack 2
.locals init (int32 v0)
	ldsfld class Test_Init_Int32 Test_Init_Int32::global
	ldloca.s v0
	call int32 Test_Init_Int32::IdentAddr(int32&)
	stfld int32 Test_Init_Int32::_ret
	ret
}

.method public static int32 Main() {
.entrypoint
.maxstack  5
.locals (int32)

	call int32 Test_Init_Int32::Test1()
	brfalse t2

	ldc.i4.1
	br FAIL

t2:
	call int32 Test_Init_Int32::Test2()
	ldc.i4.s 11
	beq.s t3

	ldc.i4.2
	br FAIL

t3:
	call int32 Test_Init_Int32::Test3()
	brfalse t4

	ldc.i4.3
	br FAIL

t4:
	call int32 Test_Init_Int32::Test4()
	brfalse t5

	ldc.i4.4
	br FAIL

t5:
	ldc.i4.0
	stloc.0
	ldloca.s 0
	call void Test_Init_Int32::Test5(int32&)
	ldloc.0
	brfalse.s t6

	ldc.i4.5
	br FAIL

t6:
	ldloca.s 0
	call void Test_Init_Int32::Test6(int32&)
	ldloc.0
	brfalse.s t7

	ldc.i4.6
	br FAIL

t7:
	call void Test_Init_Int32::Test7()
	ldsfld class Test_Init_Int32 Test_Init_Int32::global
	ldfld int32 Test_Init_Int32::_ret
	brfalse.s t8

	ldc.i4.7
	br FAIL

t8:
	call void Test_Init_Int32::Test8()
	ldsfld class Test_Init_Int32 Test_Init_Int32::global
	ldfld int32 Test_Init_Int32::_ret
	ldc.i4.s 11
	beq.s t9

	ldc.i4.8
	br FAIL

t9:
	call void Test_Init_Int32::Test9()
	ldsfld class Test_Init_Int32 Test_Init_Int32::global
	ldfld int32 Test_Init_Int32::_ret
	brfalse.s t10

	ldc.i4.s 9
	br FAIL

t10:
	call void Test_Init_Int32::Test10()
	ldsfld class Test_Init_Int32 Test_Init_Int32::global
	ldfld int32 Test_Init_Int32::_ret
	brfalse.s t11

	ldc.i4.s 10
	br FAIL

t11:

PASS:
	ldc.i4 0x0000
	ret
FAIL:
	ret
}
}