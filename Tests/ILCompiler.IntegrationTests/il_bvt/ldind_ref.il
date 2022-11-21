.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}

.class public ldind_ref
{

.method public void ldind_ref()
{
.maxstack 0
	ret
}

.method public static int32 Main() {
.entrypoint
.maxstack  2
.locals (class ldind_ref)

	newobj instance void ldind_ref::ldind_ref()
	stloc 0
	ldloca 0
	ldind.ref
	ldloc.0
	ceq
	brfalse FAIL

PASS:
	ldc.i4 0x0000
	ret
FAIL:
	ldc.i4 0x0001
	ret
}
}