.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}

.class public sealed _struct extends System.ValueType
{
	.field public int32 int32_Field
	.field public static int32 int32_SField
	.field public int32* int32_ptr_Field
	.field public static int32* int32_ptr_SField

	.method public void .ctor()
	{
		ret
	}
}

.class _initobj {

.field public static int32 DATA

.method public static int32 Main() {
.entrypoint
.locals (value class _struct, int32)
.maxstack 10

	ldloca 0
	initobj _struct

		ldloca 0
		ldc.i4 0xAB
		stfld int32 _struct::int32_Field

		ldc.i4 0xCD
		stsfld int32 _struct::int32_SField

		ldloca 0
		ldloca 1
		stfld int32* _struct::int32_ptr_Field


	ldloca 1
	stsfld int32* _struct::int32_ptr_SField

		ldloca 0
		ldfld int32 _struct::int32_Field
		ldc.i4 0xAB
		bne.un fail

		ldsfld int32 _struct::int32_SField
		ldc.i4 0xCD
		bne.un fail

		ldloca 0
		ldfld int32* _struct::int32_ptr_Field
		ldloca 1
		bne.un fail

		ldsfld int32* _struct::int32_ptr_SField
		ldloca 1
		bne.un fail

	ldloca 0
	initobj _struct

		ldloca 0
		ldfld int32 _struct::int32_Field
		ldc.i4 0x00
		bne.un fail

		ldsfld int32 _struct::int32_SField
		ldc.i4 0xCD
		bne.un fail

		ldloca 0
		ldfld int32* _struct::int32_ptr_Field
		ldnull
		bne.un fail

		ldsfld int32* _struct::int32_ptr_SField
		ldloca 1
		bne.un fail

pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}
}