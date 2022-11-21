.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}

.class public stind_ref
{

.method public void stind_ref()
{
.maxstack 0
	ret
}

.method public static int32 Main() {
.entrypoint
.maxstack  2
.locals (class stind_ref, class stind_ref)

	newobj instance void stind_ref::stind_ref()
	stloc 1

	ldloca 0
	ldloc 1
	stind.ref

	ldloc.1
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