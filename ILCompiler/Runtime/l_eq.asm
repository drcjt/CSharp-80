; 32 bit signed equal comparison
; Args from Stack, DEHL == BCAF
; Carry set if true

l_eq:
	POP DE
	EXX

	POP HL
	POP DE

	POP BC
	LD A, C
	CP L
	JR NZ, l_eq_1

	LD A, B
	CP H
	JR NZ, l_eq_1

	POP BC

	LD A, C
	CP E
	JR NZ, l_eq_2

	LD A, B
	CP D
	JR NZ, l_eq_2

l_eq_3:
	SCF

	JP l_eq_4

l_eq_1:

	POP BC

l_eq_2:

	XOR A

l_eq_4
	EXX
	PUSH DE

	RET
