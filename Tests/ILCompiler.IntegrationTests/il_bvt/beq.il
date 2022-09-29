.assembly extern System.Private.CoreLib as mscorlib
{
    .ver 0:0:0:0
}
.assembly TestAssembly
{
}
.method public static int32 Main() {
.entrypoint
.locals ()
	ldc.i4 0xFFAAFFAA
	ldc.i4 0xFFAAFFAA
	beq next0
	br fail
next0:
	ldc.i4 0xDDDD0000
	ldc.i4 0x0000DDDD
	beq fail
	br pass
pass:
	ldc.i4 0x0000
	br end
fail:
	ldc.i4 0x0001
	br end
end:
	ret
}