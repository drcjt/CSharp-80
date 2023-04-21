; 32 bit signed not equals comparison
; Args from Stack, DEHL != BCAF
; Carry set if true

i_neq:
	POP HL
	EXX

	POP HL
	POP DE

	POP BC
	LD A, C
	CP L
	JR NZ, i_neq_1

	LD A, B
	CP H
	JR NZ, i_neq_1

	POP BC

	LD A, C
	CP E
	JR NZ, i_neq_2

	LD A, B
	CP D
	JR NZ, i_neq_2

	JP i_neq_3

i_neq_1:

	POP BC

i_neq_2:
	SCF

i_neq_3
	EXX
	JP (HL)
