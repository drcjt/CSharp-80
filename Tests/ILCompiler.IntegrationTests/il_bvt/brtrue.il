﻿.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.method public static int32 Main() {
.entrypoint
.locals ()
	ldc.i4 0x1
	brtrue next0
	br fail

next0:
	ldc.i4 0x2
	brtrue pass

	ldnull
	brtrue fail

	ldc.i4 0x1
	conv.i
	brtrue next1
	br fail

next1:
	ldc.i4 0x2
	brtrue pass

fail:
	ldc.i4 0x0001
	ret
pass:
	ldc.i4 0x0000
	ret
}