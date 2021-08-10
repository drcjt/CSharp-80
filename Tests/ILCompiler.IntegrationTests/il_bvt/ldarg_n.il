.assembly extern mscorlib
{
    .ver 4:0:0:0
    .publickeytoken = (B7 7A 5C 56 19 34 E0 89)
}
.class ldarg_n {
.method public static int32 I4_0(int32,int32,int32,int32) {
	ldarg.0
	ret
}
.method public static int32 I4_1(int32,int32,int32,int32) {
	ldarg.1
	ret
}
.method public static int32 I4_2(int32,int32,int32,int32) {
	ldarg.2
	ret
}
.method public static int32 I4_3(int32,int32,int32,int32) {
	ldarg.3
	ret
}
.method public static int16 I2_0(int16,int16,int16,int16) {
	ldarg.0
	ret
}
.method public static int16 I2_1(int16,int16,int16,int16) {
	ldarg.1
	ret
}
.method public static int16 I2_2(int16,int16,int16,int16) {
	ldarg.2
	ret
}
.method public static int16 I2_3(int16,int16,int16,int16) {
	ldarg.3
	ret
}
.method public static int32 Main() {
.entrypoint
.maxstack 10
	ldc.i4.0
	ldc.i4.1
	ldc.i4.2
	ldc.i4.3
	call int32 ldarg_n::I4_0(int32,int32,int32,int32)
	ldc.i4.0
	ceq
	brfalse fail
	ldc.i4.0
	ldc.i4.1
	ldc.i4.2
	ldc.i4.3
	call int32 ldarg_n::I4_1(int32,int32,int32,int32)
	ldc.i4.1
	ceq
	brfalse fail
	ldc.i4.0
	ldc.i4.1
	ldc.i4.2
	ldc.i4.3
	call int32 ldarg_n::I4_2(int32,int32,int32,int32)
	ldc.i4.2
	ceq
	brfalse fail
	ldc.i4.0
	ldc.i4.1
	ldc.i4.2
	ldc.i4.3
	call int32 ldarg_n::I4_3(int32,int32,int32,int32)
	ldc.i4.3
	ceq
	brfalse fail
	ldc.i4.0
	conv.i2
	ldc.i4.1
	conv.i2
	ldc.i4.2
	conv.i2
	ldc.i4.3
	conv.i2
	call int16 ldarg_n::I2_0(int16,int16,int16,int16)
	ldc.i4.0
	conv.i2
	ceq
	brfalse fail
	ldc.i4.0
	conv.i2
	ldc.i4.1
	conv.i2
	ldc.i4.2
	conv.i2
	ldc.i4.3
	conv.i2
	call int16 ldarg_n::I2_1(int16,int16,int16,int16)
	ldc.i4.1
	conv.i2
	ceq
	brfalse fail
	ldc.i4.0
	conv.i2
	ldc.i4.1
	conv.i2
	ldc.i4.2
	conv.i2
	ldc.i4.3
	conv.i2
	call int16 ldarg_n::I2_2(int16,int16,int16,int16)
	ldc.i4.2
	conv.i2
	ceq
	brfalse fail
	ldc.i4.0
	conv.i2
	ldc.i4.1
	conv.i2
	ldc.i4.2
	conv.i2
	ldc.i4.3
	conv.i2
	call int16 ldarg_n::I2_3(int16,int16,int16,int16)
	ldc.i4.3
	conv.i2
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