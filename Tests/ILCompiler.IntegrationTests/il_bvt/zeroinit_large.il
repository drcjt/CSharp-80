.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.class public _zeroinit_large {
.method public static int32 Main() {
.entrypoint
.maxstack 10
.locals init (bool passed, int32* buffer, int32 n, int32 sizeToTest)

	// initialize passed to true
	ldc.i4.1
	stloc.0

	// No need to dirty stack as Z80TestRunner does this for us

	// Allocate stack space
	ldc.i4.4
	ldc.i4 0x1000
	mul
	localloc
	stloc.1

	// Set size to test
	ldc.i4 0x1000
	stloc.3

	// Start testing at 0th element allocated
	ldc.i4.0
	stloc.2
	
	// start test
	br.s loop2

loop1:
	// Calculate address of nth element being tested
	ldloc.1
	ldc.i4.4
	ldloc.2
	mul
	add

	// Get value of nth element in stack space
	ldind.i4

	// If value is zero then test passed
	brfalse.s ok

	// Test failed set passed to 0
	ldc.i4.0
	stloc.0
ok:
	// n = n + 1
	ldloc.2
	ldc.i4.1
	add
	stloc.2

loop2:
	// if (n < sizeToTest) goto loop1
	ldloc.2
	ldloc.3
	blt.s loop1

	// result is based on passed
	ldloc.0
	brfalse.s fail

pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}
}