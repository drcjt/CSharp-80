.assembly extern legacy library mscorlib {}


.class public _div {
.method public static	int32 _div(int32,int32) {
.maxstack		3
try_start:
	ldarg	0
	ldarg	1
	div
	ret
}
.method public static	int32 main() {
.entrypoint
.maxstack		20
	ldc.i4		0x80000000
	ldc.i4		0x80000000
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000001
	ceq
	brfalse		FAIL


	/* 2147483648/-1 should throw ArithmeticException as it is not representable in the result type	
	ldc.i4		0x80000000
	ldc.i4		0xFFFFFFFF
	call	int32 _div::_div(int32,int32)
	ldc.i4		0xAE
	ceq
	brfalse		FAIL
	*/

	/* 0/0 should throw DivideByZeroException
	ldc.i4		0x80000000
	ldc.i4		0x00000000
	call	int32 _div::_div(int32,int32)
	ldc.i4		0xDB0E
	ceq
	brfalse		FAIL
	*/

	ldc.i4		0x80000000
	ldc.i4		0x00000001
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x80000000
	ceq
	brfalse		FAIL

	ldc.i4		0x80000000
	ldc.i4		0x7FFFFFFF
	call	int32 _div::_div(int32,int32)
	ldc.i4		0xFFFFFFFF
	ceq
	brfalse		FAIL

	ldc.i4		0x80000000
	ldc.i4		0x55555555
	call	int32 _div::_div(int32,int32)
	ldc.i4		0xFFFFFFFF
	ceq
	brfalse		FAIL

	ldc.i4		0x80000000
	ldc.i4		0xAAAAAAAA
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000001
	ceq
	brfalse		FAIL

	ldc.i4		0xFFFFFFFF
	ldc.i4		0x80000000
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000000		
	ceq
	brfalse		FAIL
	ldc.i4		0xFFFFFFFF
	ldc.i4		0xFFFFFFFF
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000001
	ceq
	brfalse		FAIL

	/* -1/0 should throw DivideByZeroException
	ldc.i4		0xFFFFFFFF
	ldc.i4		0x00000000
	call	int32 _div::_div(int32,int32)
	ldc.i4		0xDB0E
	ceq
	brfalse		FAIL
	*/

	ldc.i4		0xFFFFFFFF
	ldc.i4		0x00000001
	call	int32 _div::_div(int32,int32)
	ldc.i4		0xFFFFFFFF
	ceq
	brfalse		FAIL
	ldc.i4		0xFFFFFFFF
	ldc.i4		0x7FFFFFFF
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000000
	ceq
	brfalse		FAIL
	ldc.i4		0xFFFFFFFF
	ldc.i4		0x55555555
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000000
	ceq
	brfalse		FAIL
	ldc.i4		0xFFFFFFFF
	ldc.i4		0xAAAAAAAA
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000000
	ceq
	brfalse		FAIL

	ldc.i4		0x00000000
	ldc.i4		0x80000000
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000000
	ceq
	brfalse		FAIL
	ldc.i4		0x00000000
	ldc.i4		0xFFFFFFFF
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000000
	ceq
	brfalse		FAIL
	/* 0/0 should throw DivideByZeroException
	ldc.i4		0x00000000
	ldc.i4		0x00000000
	call	int32 _div::_div(int32,int32)
	ldc.i4		0xDB0E
	ceq
	brfalse		FAIL
	*/
	ldc.i4		0x00000000
	ldc.i4		0x00000001
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000000
	ceq
	brfalse		FAIL
	ldc.i4		0x00000000
	ldc.i4		0x7FFFFFFF
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000000
	ceq
	brfalse		FAIL
	ldc.i4		0x00000000
	ldc.i4		0x55555555
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000000
	ceq
	brfalse		FAIL
	ldc.i4		0x00000000
	ldc.i4		0xAAAAAAAA
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000000
	ceq
	brfalse		FAIL

	ldc.i4		0x00000001
	ldc.i4		0x80000000
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000000
	ceq
	brfalse		FAIL
	ldc.i4		0x00000001
	ldc.i4		0xFFFFFFFF
	call	int32 _div::_div(int32,int32)
	ldc.i4		0xFFFFFFFF
	ceq
	brfalse		FAIL
	/* 1/0 should throw DivideByZeroException
	ldc.i4		0x00000001
	ldc.i4		0x00000000
	call	int32 _div::_div(int32,int32)
	ldc.i4		0xDB0E
	ceq
	brfalse		FAIL
	*/
	ldc.i4		0x00000001
	ldc.i4		0x00000001
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000001
	ceq
	brfalse		FAIL
	ldc.i4		0x00000001
	ldc.i4		0x7FFFFFFF
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000000
	ceq
	brfalse		FAIL
	ldc.i4		0x00000001
	ldc.i4		0x55555555
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000000
	ceq
	brfalse		FAIL
	ldc.i4		0x00000001
	ldc.i4		0xAAAAAAAA
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000000
	ceq
	brfalse		FAIL

	ldc.i4		0x7FFFFFFF
	ldc.i4		0x80000000
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000000
	ceq
	brfalse		FAIL
	ldc.i4		0x7FFFFFFF
	ldc.i4		0xFFFFFFFF
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x80000001
	ceq
	brfalse		FAIL
	/* 2147483632/0 should throw a DivideByZeroException
	ldc.i4		0x7FFFFFFF
	ldc.i4		0x00000000
	call	int32 _div::_div(int32,int32)
	ldc.i4		0xDB0E
	ceq
	brfalse		FAIL
	*/
	ldc.i4		0x7FFFFFFF
	ldc.i4		0x00000001
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x7FFFFFFF
	ceq
	brfalse		FAIL
	ldc.i4		0x7FFFFFFF
	ldc.i4		0x7FFFFFFF
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000001
	ceq
	brfalse		FAIL
	ldc.i4		0x7FFFFFFF
	ldc.i4		0x55555555
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000001
	ceq
	brfalse		FAIL
	ldc.i4		0x7FFFFFFF
	ldc.i4		0xAAAAAAAA
	call	int32 _div::_div(int32,int32)
	ldc.i4		0xFFFFFFFF
	ceq
	brfalse		FAIL

	ldc.i4		0x55555555
	ldc.i4		0x80000000
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000000
	ceq
	brfalse		FAIL
	ldc.i4		0x55555555
	ldc.i4		0xFFFFFFFF
	call	int32 _div::_div(int32,int32)
	ldc.i4		0xAAAAAAAB
	ceq
	brfalse		FAIL
	/* 1431655765/0 should throw a DivideByZeroException
	ldc.i4		0x55555555
	ldc.i4		0x00000000
	call	int32 _div::_div(int32,int32)
	ldc.i4		0xDB0E
	ceq
	brfalse		FAIL
	*/
	ldc.i4		0x55555555
	ldc.i4		0x00000001
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x55555555
	ceq
	brfalse		FAIL
	ldc.i4		0x55555555
	ldc.i4		0x7FFFFFFF
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000000
	ceq
	brfalse		FAIL
	ldc.i4		0x55555555
	ldc.i4		0x55555555
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000001
	ceq
	brfalse		FAIL
	ldc.i4		0x55555555
	ldc.i4		0xAAAAAAAA
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000000
	ceq
	brfalse		FAIL

	ldc.i4		0xAAAAAAAA
	ldc.i4		0x80000000
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000000
	ceq
	brfalse		FAIL
	ldc.i4		0xAAAAAAAA
	ldc.i4		0xFFFFFFFF
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x55555556
	ceq
	brfalse		FAIL
	/* -1431655766/0 should throw a DivideByZeroException
	ldc.i4		0xAAAAAAAA
	ldc.i4		0x00000000
	call	int32 _div::_div(int32,int32)
	ldc.i4		0xDB0E
	ceq
	brfalse		FAIL
	*/
	ldc.i4		0xAAAAAAAA
	ldc.i4		0x00000001
	call	int32 _div::_div(int32,int32)
	ldc.i4		0xAAAAAAAA
	ceq
	brfalse		FAIL
	ldc.i4		0xAAAAAAAA
	ldc.i4		0x7FFFFFFF
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000000
	ceq
	brfalse		FAIL
	ldc.i4		0xAAAAAAAA
	ldc.i4		0x55555555
	call	int32 _div::_div(int32,int32)
	ldc.i4		0xFFFFFFFF
	ceq
	brfalse		FAIL
	ldc.i4		0xAAAAAAAA
	ldc.i4		0xAAAAAAAA
	call	int32 _div::_div(int32,int32)
	ldc.i4		0x00000001
	ceq
	brfalse		FAIL

	ldc.i4 0x0000
	ret
FAIL:
	ldc.i4	0x0001
	ret
}
}
.assembly div_i4{}