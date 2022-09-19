.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.class public _switch {
.method public static int32 Main() {
.entrypoint
.maxstack 10
.locals (int32)
	ldc.i4	0x0
	stloc 0
	ldc.i4 0x0

	TAG:
	switch (TC1, TC2, TC3)
	ldloc 0
	ldc.i4 0x3
	ceq
	brfalse fail
	br pass

	TC1:
		ldloc 0
		ldc.i4 0x1
		add
		stloc 0
	
		ldc.i4 0x1
		br TAG

	TC2:
		ldloc 0
		ldc.i4 0x1
		add
		stloc 0

		ldc.i4 0x2
		br TAG

	TC3:
		ldloc 0
		ldc.i4 0x1
		add
		stloc 0

		ldc.i4 0x3
		br TAG

	br fail

pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}
}