.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
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

.class Program 
{
	.method public static int32 Main() 
	{
		.entrypoint
		.maxstack 10
		.locals init 
		(
			[0] valuetype SimpleStruct 'vector',
			[1] int32
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

	pass:
		ldc.i4 0x0000
		ret
	fail:
		ldc.i4 0x0001
		ret
	}
}