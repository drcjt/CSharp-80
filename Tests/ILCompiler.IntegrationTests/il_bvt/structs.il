.assembly extern mscorlib
{
    .ver 4:0:0:0
    .publickeytoken = (B7 7A 5C 56 19 34 E0 89)
}
.assembly structsexe
{
}

.class public sealed _struct extends System.ValueType
{
	.field public int32 X
	.field public int32 Y

	.method public instance void .ctor (int32 x, int32 y)
	{
		.maxstack 8
		ldarg.0
		ldarg.1
		stfld int32 _struct::X
		ldarg.0
		ldarg.2
		stfld int32 _struct::Y
		ret
	}
}

.class public _structs {
.method public static valuetype _struct _makestruct(int32 x, int32 y) {
.maxstack 2
	ldarg.0
	ldarg.1
	newobj instance void _struct::.ctor(int32, int32)
	stloc.0
	ldloc.0
	ret
}
.method public static int32 Main() {
.entrypoint
.maxstack 10
	ldc.i4.3
	ldc.i4.4
	call valuetype _struct _structs::_makestruct(int32, int32)
	stloc.0
	ldloc.0
	ldfld int32 _struct::X
	ldc.i4.3
	ceq
	brfalse fail
	ldloc.0
	ldfld int32 _struct::Y
	ldc.i4.4
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