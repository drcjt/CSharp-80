.assembly extern mscorlib
{
    .ver 4:0:0:0
    .publickeytoken = (B7 7A 5C 56 19 34 E0 89)
}
.class field_tests {
.method public static int32 Main() {
.entrypoint
.maxstack 10
.locals (int8, int16, int32)

//	TODO: Need to work out what can be done here

pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}
}