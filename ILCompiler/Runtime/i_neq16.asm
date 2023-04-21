; 16 bit signed not equals comparison
; Args from Stack, DEHL != BCAF
; Carry set if true

i_neq16:
	POP HL		; Save return address

	POP BC
	POP DE

	LD A, C
	CP E
	JR NZ, i_neq16_1

	LD A, B
	CP D
	JR NZ, i_neq16_1

	JP i_neq16_2

i_neq16_1:
	SCF

i_neq16_2
	JP (HL)
