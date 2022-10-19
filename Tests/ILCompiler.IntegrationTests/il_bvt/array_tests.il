.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}

.class public Test {
.field public int32 I1
.method public void .ctor(int32) {
.maxstack 10
	ldarg 0
	call instance void [mscorlib]System.Object::.ctor()
	ldarg 0
	ldarg 1
	stfld int32 Test::I1
	ret
}
}

.class public array_tests
{
.method public static int32 I8(int32, int32, int32) {
.maxstack  5
.locals (int8[])

	ldc.i4 0x00000004
	newarr [mscorlib]System.Byte
	stloc 0

	ldloc 0
	ldarg.0
	ldarg.1
	stelem.i1

	ldloc 0
	ldarg.0
	ldelem.i1

	ldarg.2
	ceq
	ret
}

.method public static int32 U8(int32, uint32, uint32) {
.maxstack  5
.locals (int8[])

	ldc.i4 0x00000004
	newarr [mscorlib]System.Byte
	stloc 0

	ldloc 0
	ldarg.0
	ldarg.1
	stelem.i1

	ldloc 0
	ldarg.0
	ldelem.u1

	ldarg.2
	ceq
	ret
}


.method public static int32 I(int32, int32, int32) {
.maxstack  5
.locals (int16[])

	ldc.i4 0x00000004
	newarr [mscorlib]System.Int16
	stloc 0

	ldloc 0
	ldarg.0
	ldarg.1
	conv.i
	stelem.i

	ldloc 0
	ldarg.0
	ldelem.i

	ldarg.2
	conv.i
	ceq
	ret
}

.method public static int32 I16(int32, int32, int32) {
.maxstack  5
.locals (int16[])

	ldc.i4 0x00000004
	newarr [mscorlib]System.Int16
	stloc 0

	ldloc 0
	ldarg.0
	ldarg.1
	stelem.i2

	ldloc 0
	ldarg.0
	ldelem.i2

	ldarg.2
	ceq
	ret
}

.method public static int32 U16(int32, uint32, uint32) {
.maxstack  5
.locals (int16[])

	ldc.i4 0x00000004
	newarr [mscorlib]System.Int16
	stloc 0

	ldloc 0
	ldarg.0
	ldarg.1
	stelem.i2

	ldloc 0
	ldarg.0
	ldelem.u2

	ldarg.2
	ceq
	ret
}

.method public static int32 I32(int32, int32, int32) {
.maxstack  5
.locals (int32[])

	ldc.i4 0x00000004
	newarr [mscorlib]System.Int32
	stloc 0

	ldloc 0
	ldarg.0
	ldarg.1
	stelem.i4

	ldloc 0
	ldarg.0
	ldelem.i4

	ldarg.2
	ceq
	ret
}

.method public static int32 U32(int32, uint32, uint32) {
.maxstack  5
.locals (int32[])

	ldc.i4 0x00000004
	newarr [mscorlib]System.Int32
	stloc 0

	ldloc 0
	ldarg.0
	ldarg.1
	stelem.i4

	ldloc 0
	ldarg.0
	ldelem.u4

	ldarg.2
	ceq
	ret
}

.method public static int32 RefTest(int32, int32) {
.maxstack  5
.locals (class Test[], class Test)

	ldc.i4 0x00000004
	newarr Test
	stloc.0

	ldarg.1
	newobj instance void Test::.ctor(int32)
	stloc.1

	ldloc.0
	ldarg.0
	ldloc.1
	stelem.ref

	ldloc.0
	ldarg.0
	ldelem.ref

	ldfld int32 Test::I1
	ldarg.1
	ceq
	ret
}

.method public static int32 Main() {
.entrypoint
.maxstack  5
.locals ()

	ldc.i4 0x00
	ldc.i4 0x72
	call int32 array_tests::RefTest(int32, int32)
	brfalse FAIL

	ldc.i4 0x00
	ldc.i4 0x25
	ldc.i4 0x25
	call int32 array_tests::I8(int32, int32, int32)
	brfalse FAIL

	ldc.i4 0x01
	ldc.i4 0xff
	ldc.i4 0xffffffff	// negative value is sign extended
	call int32 array_tests::I8(int32, int32, int32)
	brfalse FAIL

	ldc.i4 0x00
	ldc.i4 0x25
	ldc.i4 0x25
	call int32 array_tests::U8(int32, uint32, uint32)
	brfalse FAIL

	ldc.i4 0x01
	ldc.i4 0xff
	ldc.i4 0xff
	call int32 array_tests::U8(int32, uint32, uint32)
	brfalse FAIL

	ldc.i4 0x00
	ldc.i4 0x00007fff
	ldc.i4 0x00007fff
	call int32 array_tests::I16(int32, int32, int32)
	brfalse FAIL

	ldc.i4 0x01
	ldc.i4 0x0000ffff
	ldc.i4 0xffffffff	// negative value is sign extended
	call int32 array_tests::I16(int32, int32, int32)
	brfalse FAIL

	ldc.i4 0x00
	ldc.i4 0x00007fff
	ldc.i4 0x00007fff
	call int32 array_tests::U16(int32, uint32, uint32)
	brfalse FAIL

	ldc.i4 0x01
	ldc.i4 0x0000ffff
	ldc.i4 0x0000ffff
	call int32 array_tests::U16(int32, uint32, uint32)
	brfalse FAIL

	ldc.i4 0x00
	ldc.i4 0x00007fff
	ldc.i4 0x00007fff
	call int32 array_tests::I(int32, int32, int32)
	brfalse FAIL

	ldc.i4 0x01
	ldc.i4 0x0000ffff
	ldc.i4 0x0000ffff
	call int32 array_tests::I(int32, int32, int32)
	brfalse FAIL

	ldc.i4 0x00
	ldc.i4 0x7fffffff
	ldc.i4 0x7fffffff
	call int32 array_tests::I32(int32, int32, int32)
	brfalse FAIL

	ldc.i4 0x01
	ldc.i4 0x80000000
	ldc.i4 0x80000000
	call int32 array_tests::I32(int32, int32, int32)
	brfalse FAIL

	ldc.i4 0x00
	ldc.i4 0x7fffffff
	ldc.i4 0x7fffffff
	call int32 array_tests::U32(int32, uint32, uint32)
	brfalse FAIL

	ldc.i4 0x01
	ldc.i4 0x80000000
	ldc.i4 0x80000000
	call int32 array_tests::U32(int32, uint32, uint32)
	brfalse FAIL

PASS:
	ldc.i4 0x0000
	ret
FAIL:
	ldc.i4 0x0001
	ret
}
}