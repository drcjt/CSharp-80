; 32 bit signed equal comparison
; Args from Stack, DEHL == BCAF
; Carry set if true

i_eq:
	POP DE
	EXX

	POP HL
	POP DE

	POP BC
	LD A, C
	CP L
	JR NZ, i_eq_1

	LD A, B
	CP H
	JR NZ, i_eq_1

	POP BC

	LD A, C
	CP E
	JR NZ, i_eq_2

	LD A, B
	CP D
	JR NZ, i_eq_2

	SCF

	JP i_eq_3

i_eq_1:

	POP BC

i_eq_2:

	XOR A

i_eq_3
	EXX
	PUSH DE

	RET
