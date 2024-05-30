.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly fielda_tests
{
}

.class public fielda_tests 
{
	.field public static int8 i1SField
	.field public static int16 i2SField
	.field public static int32 i4SField

	.method public void .ctor() { 
		.maxstack 10
		ldarg.0
		call instance void [mscorlib]System.Object::.ctor()
		ret
	}

	.method public void initialize() {
	.maxstack 10

		ldc.i4 0x1
		stsfld int8 fielda_tests::i1SField

		ldc.i4 0x2
		stsfld int16 fielda_tests::i2SField

		ldc.i4 0x4
		stsfld int32 fielda_tests::i4SField

		ret
	}

	.method public static int32 Main() 
	{
		.entrypoint
		.maxstack 10
		.locals init 
		(
			[0] class fielda_tests
		)

	   newobj instance void fielda_tests::.ctor()
	   dup
	   stloc 0
	   call instance void fielda_tests::initialize()

	   ldsflda int8 fielda_tests::i1SField
	   ldind.i1
	   ldc.i4 0x1
	   ceq
	   brfalse fail

	   ldsflda int16 fielda_tests::i2SField
	   ldind.i2
	   ldc.i4 0x2
	   ceq
	   brfalse fail

	   ldsflda int32 fielda_tests::i4SField
	   ldind.i4
	   ldc.i4 0x4
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