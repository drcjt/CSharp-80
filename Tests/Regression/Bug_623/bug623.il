.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}

.assembly TestAssembly
{
	.ver 0:0:0:0
}

.class public Bug623
{

.method public static int32 Foo(bool b, int32 i)
{
.maxstack 5
.locals (int32)
	
	ldc.i4 1
	stloc.0

	ldarg.0
	brtrue bb1

	ldc.i4 0
	br.s bb2

bb1:
	ldc.i4 0
	stloc.0

	ldloc.0
	
bb2:	
	ret
}

.method public static int32 Main() {
.entrypoint
.maxstack  5
.locals (int32)

	ldc.i4 1
	ldc.i4 0

	call int32 Bug623::Foo(bool,int32)

	ret
}
}