; 16 bit signed not equals comparison
; Args from Stack, DEHL != BCAF
; Carry set if true

i_neq16:
	POP BC		; Save return address

	POP HL
	POP DE

	LD A, L
	CP E
	JR NZ, i_neq16_1

	LD A, H
	CP D
	JR NZ, i_neq16_1

	JP i_neq16_2

i_neq16_1:
	SCF

i_neq16_2
	PUSH BC
	RET
