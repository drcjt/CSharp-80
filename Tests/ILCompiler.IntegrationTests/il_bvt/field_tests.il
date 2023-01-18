.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly field_tests
{
}

.class public sequential ansi sealed beforefieldinit Struct0 extends System.ValueType
{
	.field private bool b0;
	.field private int32 i1;
}

.class public sequential ansi sealed beforefieldinit SimpleStruct extends System.ValueType 
{
	.field public int32 X
	.field public int32 Y
	.field public int32* Ptr
}

.class public field_tests 
{
	.field public int8 i1Field
	.field public int16 i2Field
	.field public int32 i4Field
	.field public class field_tests ptrField

	.field public static int8 i1SField
	.field public static int16 i2SField
	.field public static int32 i4SField
	.field public static class field_tests ptrSField

	.method public void initialize() {
	.maxstack 10

		ldarg 0
		ldc.i4 0x1
		stfld int8 field_tests::i1Field

		ldarg 0
		ldc.i4 0x2
		stfld int16 field_tests::i2Field

		ldarg 0
		ldc.i4 0x4
		stfld int32 field_tests::i4Field

		ldarg 0
		ldarg 0
		stfld class field_tests field_tests::ptrField

		ldc.i4 0x1
		stsfld int8 field_tests::i1SField

		ldc.i4 0x2
		stsfld int16 field_tests::i2SField

		ldc.i4 0x4
		stsfld int32 field_tests::i4SField

		ldarg 0
		stsfld class field_tests field_tests::ptrSField

		ret
	}

	.method public static int32 Main() 
	{
		.entrypoint
		.maxstack 10
		.locals init 
		(
			[0] valuetype SimpleStruct 'vector',
			[1] int32,
			[2] class field_tests
		)

	   ldloca.s 0
	   ldc.i4.3
	   stfld int32 SimpleStruct::X

	   ldloca.s 0
	   ldc.i4.4
	   stfld int32 SimpleStruct::Y

	   ldc.i4 25
	   stloc.1

	   ldloca.s 0
	   ldloca.s 1
	   stfld int32* SimpleStruct::Ptr   

	   ldloc.0
	   ldfld int32 SimpleStruct::X
	   ldc.i4.3
	   ceq
	   brfalse fail

	   ldloc.0
	   ldfld int32 SimpleStruct::Y
	   ldc.i4.4
	   ceq
	   brfalse fail

	   ldloc.0
	   ldfld int32* SimpleStruct::Ptr
	   ldind.i4
	   ldloc.1
	   ceq
	   brfalse fail

	   newobj instance void field_tests::.ctor()
	   dup
	   stloc 2
	   call instance void field_tests::initialize()
	   ldloc 2
	   ldfld int8 field_tests::i1Field
	   ldc.i4 0x1
	   ceq
	   brfalse fail

	   ldloc 2
	   ldfld int16 field_tests::i2Field
	   ldc.i4 0x2
	   ceq
	   brfalse fail

	   ldloc 2
	   ldfld int32 field_tests::i4Field
	   ldc.i4 0x4
	   ceq
	   brfalse fail

	   /* TODO: Fix this when isinst is implemented
	   ldloc 2
	   ldfld class field_tests field_tests::ptrField
	   isinst field_tests
	   brfalse fail
	   */

	   ldsfld int8 field_tests::i1SField
	   ldc.i4 0x1
	   ceq
	   brfalse fail

	   ldsfld int16 field_tests::i2SField
	   ldc.i4 0x2
	   ceq
	   brfalse fail

	   ldsfld int32 field_tests::i4SField
	   ldc.i4 0x4
	   ceq
	   brfalse fail

	   /* TODO: Fix this when isinst is implemented
	   ldsfld class field_tests field_tests::ptrSField
	   isinst field_tests
	   brfalse fail
	   */

	pass:
		ldc.i4 0x0000
		ret
	fail:
		ldc.i4 0x0001
		ret
	}
}