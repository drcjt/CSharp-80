; Logical Xor of two 32 bit values on stack
; Leaves result on the stack
;
; Uses: HL, HL', DE, BC, AF

i_xor:
	POP HL
	EXX

	POP HL
	POP DE

	POP BC

	LD A, H
	XOR B
	LD H, A

	LD A, L
	XOR C
	LD L, A

	POP BC

	LD A, D
	XOR B
	LD D, A

	LD A, E
	XOR C
	LD E, A

	PUSH DE
	PUSH HL

	EXX
	JP (HL)
