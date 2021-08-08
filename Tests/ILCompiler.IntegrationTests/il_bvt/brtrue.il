.assembly brtrueexe
{
}
.method public static int32 Main() {
.entrypoint
.locals ()
	ldc.i4 0x1
	brtrue pass
fail:
	ldc.i4 0x0001
	ret
pass:
	ldc.i4 0x0000
	ret
}