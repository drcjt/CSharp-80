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
	ldc.i4 0x44444444
	ldc.i4 0x55555555
	bne.un next0
	br fail
next0:
	ldc.i4 0x66666666
	ldc.i4 0x66666666
	bne.un fail
	br next1
next1:
	ldc.i4 0x55555555
	ldc.i4 0x44444444
	bne.un pass
	br fail
pass:
	ldc.i4 0x0000
	br end
fail:
	ldc.i4 0x0001
	br end
end:
	ret
}