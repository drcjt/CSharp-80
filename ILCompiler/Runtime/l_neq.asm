; 32 bit signed not equals comparison
; Args from Stack, DEHL != BCAF
; Carry set if true

l_neq:
	POP DE
	EXX

	POP HL
	POP DE

	POP BC
	LD A, C
	CP L
	JR NZ, l_neq_1

	LD A, B
	CP H
	JR NZ, l_neq_1

	POP BC

	LD A, C
	CP E
	JR NZ, l_neq_2

	LD A, B
	CP D
	JR NZ, l_neq_2

l_neq_3:
	JP l_eq_4

l_neq_1:

	POP BC

l_neq_2:
	SCF

l_neq_4
	EXX
	PUSH DE

	RET
