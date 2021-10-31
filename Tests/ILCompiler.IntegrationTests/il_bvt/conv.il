.assembly convexe
{
}
.assembly extern mscorlib
{
    .ver 4:0:0:0
    .publickeytoken = (B7 7A 5C 56 19 34 E0 89)
}
.class public conv {
.method public static int32 i4(int32) {
.maxstack 2
	ldarg 0
	ret
}
}
.method public static int32 Main() {
.entrypoint
.maxstack 10

// conv.u1 tests
	ldc.i4 0x7fffffff	// max_i4
	conv.u1
	ldc.i4 0xff
	bne.un fail

	ldc.i4 0x00000001	// one_i4
	conv.u1
	ldc.i4 0x01
	bne.un fail

	ldc.i4 0x00000000	// zero_i4
	conv.u1
	ldc.i4 0x00
	bne.un fail

	ldc.i4 0xffffffff	// minus_one_i4
	conv.u1
	ldc.i4 0x000000ff
	bne.un fail

	ldc.i4 0x80000000	// min_i4
	conv.u1
	ldc.i4 0x00
	bne.un fail

// conv.i1 tests
	ldc.i4 0x7fffffff	// max_i4
	conv.i1
	ldc.i4 0xffffffff
	bne.un fail

	ldc.i4 0x00000001	// one_i4
	conv.i1
	ldc.i4 0x01
	bne.un fail


	ldc.i4 0x00000000	// zero_i4
	conv.i1
	ldc.i4 0x00
	bne.un fail

	ldc.i4 0xffffffff	// minus_one_i4
	conv.i1
	ldc.i4 0xffffffff
	bne.un fail

	ldc.i4 0x80000000	// min_i4
	conv.i1
	ldc.i4 0x00
	bne.un fail

// conv.u2 tests
	ldc.i4 0x7fffffff	// max_i4
	conv.u2
	ldc.i4 0xffff
	bne.un fail

	ldc.i4 0x00000001	// one_i4
	conv.u2
	ldc.i4 0x0001
	bne.un fail

	ldc.i4 0x00000000	// zero_i4
	conv.u2
	ldc.i4 0x0000
	bne.un fail

	ldc.i4 0xffffffff	// minus_one_i4
	conv.u2
	ldc.i4 0x0000ffff
	bne.un fail

	ldc.i4 0x80000000	// min_i4
	conv.u2
	ldc.i4 0x0000
	bne.un fail

// conv.i2 tests
	ldc.i4 0x7fffffff	// max_i4
	conv.i2
	ldc.i4 0xffffffff
	bne.un fail

	ldc.i4 0x00000001	// one_i4
	conv.i2
	ldc.i4 0x0001
	bne.un fail

	ldc.i4 0x00000000	// zero_i4
	conv.i2
	ldc.i4 0x0000
	bne.un fail

	ldc.i4 0xffffffff	// minus_one_i4
	conv.i2
	ldc.i4 0xffffffff
	bne.un fail

	ldc.i4 0x80000000	// min_i4
	conv.i2
	ldc.i4 0x0000
	bne.un fail

	// TODO: Where did this come from?
	ldc.i4 0x8F0F0F0F
	call int32 conv::i4(int32)
	conv.i2
	ldc.i4 0x00000F0F
	ceq
	brfalse fail

// conv.u4 tests
	ldc.i4 0x7fffffff	// max_i4
	conv.u4
	ldc.i4 0x7fffffff
	bne.un fail

	ldc.i4 0x00000001	// one_i4
	conv.u4
	ldc.i4 0x00000001
	bne.un fail

	ldc.i4 0x00000000	// zero_i4
	conv.u4
	ldc.i4 0x00000000
	bne.un fail

	ldc.i4 0xffffffff	// minus_one_i4
	conv.u4
	ldc.i4 0xffffffff
	bne.un fail

	ldc.i4 0x80000000	// min_i4
	conv.u4
	ldc.i4 0x80000000
	bne.un fail

// conv.i4 tests
	ldc.i4 0x7fffffff	// max_i4
	conv.i4
	ldc.i4 0x7fffffff
	bne.un fail

	ldc.i4 0x00000001	// one_i4
	conv.i4
	ldc.i4 0x00000001
	bne.un fail

	ldc.i4 0x00000000	// zero_i4
	conv.i4
	ldc.i4 0x00000000
	bne.un fail

	ldc.i4 0xffffffff	// minus_one_i4
	conv.i4
	ldc.i4 0xffffffff
	bne.un fail

	ldc.i4 0x80000000	// min_i4
	conv.i4
	ldc.i4 0x80000000
	bne.un fail

// conv.i tests
	ldc.i4 0x7fffffff	// max_i4
	conv.i
	ldc.i4 0x7fffffff
	bne.un fail

	ldc.i4 0x00000001	// one_i4
	conv.i
	ldc.i4 0x00000001
	bne.un fail

	ldc.i4 0x00000000	// zero_i4
	conv.i
	ldc.i4 0x00000000
	bne.un fail

	ldc.i4 0xffffffff	// minus_one_i4
	conv.i
	ldc.i4 0xffffffff
	bne.un fail

	ldc.i4 0x80000000	// min_i4
	conv.i
	ldc.i4 0x80000000
	bne.un fail

// conv.u tests
	ldc.i4 0x7fffffff	// max_i4
	conv.u
	ldc.i4 0x7fffffff
	bne.un fail

	ldc.i4 0x00000001	// one_i4
	conv.u
	ldc.i4 0x00000001
	bne.un fail

	ldc.i4 0x00000000	// zero_i4
	conv.u
	ldc.i4 0x00000000
	bne.un fail

	ldc.i4 0xffffffff	// minus_one_i4
	conv.u
	ldc.i4 0xffffffff
	bne.un fail

	// TODO: Is this a valid test?
	ldc.i4 0x80000000	// min_i4
	conv.u
	ldc.i4 0x80000000
	bne.un fail

pass:
	ldc.i4 0x0000
	ret
fail:
	ldc.i4 0x0001
	ret
}