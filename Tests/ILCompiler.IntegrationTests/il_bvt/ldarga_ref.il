.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}

.class _ldarg {

.method public void _ldarg() {
.maxstack 0
	ret
}

.method public class _ldarg args(class _ldarg) {
.maxstack 5
	ldarga 0
	ldind.ref
	ret
}

.method public static int32 Main() {
.entrypoint
.locals(class _ldarg)
.maxstack 2
	newobj instance void _ldarg::_ldarg()
	stloc 0
	ldloc 0
	ldloc 0
	call instance class _ldarg _ldarg::args(class _ldarg)
	ldloc 0
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