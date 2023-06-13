.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}

.assembly TestAssembly
{
	.ver 0:0:0:0
}
.class public OutOfOrder
{
    .field private static int32 o
    .field private static int32 t1_order
    .field private static int32 t2_order

	.method public static void T1() {
		.maxstack 8
			ldsfld int32 OutOfOrder::o
			dup
			ldc.i4.1
			add
			stsfld int32 OutOfOrder::o
			stsfld int32 OutOfOrder::t1_order
			ret
	}

	.method public static char[] T2() {
		.maxstack 8
			ldsfld int32 OutOfOrder::o
			dup
			ldc.i4.1
			add
			stsfld int32 OutOfOrder::o
			stsfld int32 OutOfOrder::t2_order
			ldc.i4.5
			newarr [mscorlib]System.Char
			ret
	}

.method public static int32 Main() {
.entrypoint
.maxstack  5
.locals ()
	call char[] OutOfOrder::T2()
	call void OutOfOrder::T1()
	newobj instance void [mscorlib]System.String::.ctor(char[])
	pop
	ldsfld int32 OutOfOrder::t1_order
	ldsfld int32 OutOfOrder::t2_order
	cgt
	brfalse FAIL
PASS:
	ldc.i4 0x0000
	ret
FAIL:
	ldc.i4 0x0001
	ret
}
}