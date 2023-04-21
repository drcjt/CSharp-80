; Logical Or of two 32 bit values on stack
; Leaves result on the stack
;
; Uses: HL, HL', DE, BC, AF

i_or:
	POP HL
	EXX

	POP HL
	POP DE

	POP BC

	LD A, H
	OR B
	LD H, A

	LD A, L
	OR C
	LD L, A

	POP BC

	LD A, D
	OR B
	LD D, A

	LD A, E
	OR C
	LD E, A

	PUSH DE
	PUSH HL

	EXX
	JP (HL)
