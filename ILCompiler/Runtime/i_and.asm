; Logical And of two 32 bit values on stack
; Leaves result on the stack
;
; Uses: HL, HL', DE, BC, AF

i_and:
	EXX
	POP HL
	EXX

	POP HL
	POP DE

	POP BC

	LD A, H
	AND B
	LD H, A

	LD A, L
	AND C
	LD L, A

	POP BC

	LD A, D
	AND B
	LD D, A

	LD A, E
	AND C
	LD E, A

	PUSH DE
	PUSH HL

	EXX
	PUSH HL
	EXX
	RET
