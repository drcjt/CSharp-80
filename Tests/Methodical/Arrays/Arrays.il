.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}

.assembly TestAssembly
{
	.ver 0:0:0:0
}

.class public InitializeArray
{

.method public static int32 Main()
{
.entrypoint
.maxstack 4

    call bool InitializeArray::SZArray_Int32_5_ValidInitializationTest()
    brfalse.s fail

    call bool InitializeArray::SZArray_Int16_5_ValidInitializationTest()
    brfalse.s fail

	br pass

fail:
	ldc.i4 0x0001
	ret

pass:
	ldc.i4 0x0000
	ret
}

.method private hidebysig static bool SZArray_Int32_5_ValidInitializationTest() cil managed noinlining
{
	.maxstack 8
	call int32[] InitializeArray::SZArray_Initialize_Int32_5()
	call int32[] InitializeArray::SZArray_Initialize_Int32_WithoutInitializeArray()
	call bool InitializeArray::ArrayEquals(class [mscorlib]System.Array, class [mscorlib]System.Array)
	ret
}

.method private hidebysig static bool SZArray_Int16_5_ValidInitializationTest() cil managed noinlining
{
	.maxstack 8
	call int16[] InitializeArray::SZArray_Initialize_Int16_5()
	call int16[] InitializeArray::SZArray_Initialize_Int16_WithoutInitializeArray()
	call bool InitializeArray::ArrayEquals(class [mscorlib]System.Array, class [mscorlib]System.Array)
	ret
}


.method private static int32[] SZArray_Initialize_Int32_5()
{
	.maxstack 8
	ldc.i4 5
	newarr int32
	dup
	ldtoken field valuetype 'ArrayInitializers'/'StaticArrayInitTypeSize20' 'ArrayInitializers'::'InitializerField20'
	call void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array, valuetype[mscorlib] System.RuntimeFieldHandle)
	ret
}

.method private static int16[] SZArray_Initialize_Int16_5()
{
	.maxstack 8
	ldc.i4 5
	newarr int16
	dup
	ldtoken field valuetype 'ArrayInitializers'/'StaticArrayInitTypeSize10' 'ArrayInitializers'::'InitializerField10'
	call void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array, valuetype[mscorlib] System.RuntimeFieldHandle)
	ret
}

.method private static int32[] SZArray_Initialize_Int32_WithoutInitializeArray()
{
	.maxstack 8
	ldc.i4 5
	newarr int32
	dup
	ldc.i4.0
	ldc.i4.s	1
	stelem.i4
	dup
	ldc.i4.1
	ldc.i4.s	2
	stelem.i4
	dup
	ldc.i4.2
	ldc.i4.s	3
	stelem.i4
	dup
	ldc.i4.3
	ldc.i4.s	4
	stelem.i4
	dup
	ldc.i4.4
	ldc.i4.s	5
	stelem.i4
	ret
}

.method private static int16[] SZArray_Initialize_Int16_WithoutInitializeArray()
{
	.maxstack 8
	ldc.i4 5
	newarr int16
	dup
	ldc.i4.0
	ldc.i4.s	1
	stelem.i2
	dup
	ldc.i4.1
	ldc.i4.s	2
	stelem.i2
	dup
	ldc.i4.2
	ldc.i4.s	3
	stelem.i2
	dup
	ldc.i4.3
	ldc.i4.s	4
	stelem.i2
	dup
	ldc.i4.4
	ldc.i4.s	5
	stelem.i2
	ret
}


//
// ArrayEquals was generated from the following C# code:
//
//	static bool ArrayEquals(Array x, Array y)
//	{
//		if (x.Length != y.Length)
//			return false;
//		for (int i = 0; i < x.Length; i++)
//      {
//          if (!object.Equals(x.GetValue(i), y.GetValue(i)))
//              return false;
//      }	
//      return true;
//  }

.method private hidebysig static bool ArrayEquals (class [mscorlib]System.Array x, class [mscorlib]System.Array y) cil managed 
{
	.maxstack 3
	.locals init (
		[0] bool,
		[1] bool,
		[2] int32 i,
		[3] bool,
		[4] bool
	)

    IL_0000: nop
    IL_0001: ldarg.0
    IL_0002: callvirt instance int32 [mscorlib]System.Array::get_Length()
    IL_0007: ldarg.1
    IL_0008: callvirt instance int32 [mscorlib]System.Array::get_Length()
    IL_000d: ceq
    IL_000f: ldc.i4.0
    IL_0010: ceq
    IL_0012: stloc.0
    IL_0013: ldloc.0
    IL_0014: brfalse.s IL_001a

    IL_0016: ldc.i4.0
    IL_0017: stloc.1
    IL_0018: br.s IL_0055

    IL_001a: ldc.i4.0
    IL_001b: stloc.2
    IL_001c: br.s IL_0042
        IL_001e: nop
        IL_001f: ldarg.0
        IL_0020: ldloc.2
        IL_0021: callvirt instance object [mscorlib]System.Array::GetValue(int32)
        IL_0026: ldarg.1
        IL_0027: ldloc.2
        IL_0028: callvirt instance object [mscorlib]System.Array::GetValue(int32)
        IL_002d: call bool [mscorlib]System.Object::Equals(object, object)
        IL_0032: ldc.i4.0
        IL_0033: ceq
        IL_0035: stloc.3
        IL_0036: ldloc.3
        IL_0037: brfalse.s IL_003d

        IL_0039: ldc.i4.0
        IL_003a: stloc.1
        IL_003b: br.s IL_0055

        IL_003d: nop
        IL_003e: ldloc.2
        IL_003f: ldc.i4.1
        IL_0040: add
        IL_0041: stloc.2

        IL_0042: ldloc.2
        IL_0043: ldarg.0
        IL_0044: callvirt instance int32 [mscorlib]System.Array::get_Length()
        IL_0049: clt
        IL_004b: stloc.s 4
        IL_004d: ldloc.s 4
        IL_004f: brtrue.s IL_001e

    IL_0051: ldc.i4.1
    IL_0052: stloc.1
    IL_0053: br.s IL_0055

    IL_0055: ldloc.1
    IL_0056: ret
}
}

.class private auto ansi 'ArrayInitializers' extends [mscorlib]System.Object
{
	.class explicit ansi sealed nested private 'StaticArrayInitTypeSize10' extends [mscorlib]System.ValueType
	{
		.pack 1
		.size 10
	}

	.class explicit ansi sealed nested private 'StaticArrayInitTypeSize20' extends [mscorlib]System.ValueType
	{
		.pack 1
		.size 20
	}

	.field static assembly valuetype 'ArrayInitializers'/'StaticArrayInitTypeSize10' 'InitializerField10' at InitializerField10Data
	.field static assembly valuetype 'ArrayInitializers'/'StaticArrayInitTypeSize20' 'InitializerField20' at InitializerField20Data

	.data cil InitializerField10Data = bytearray (01 00 02 00 03 00 04 00 05 00)
	.data cil InitializerField20Data = bytearray (01 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00 05 00 00 00)
}