; Logical Or of two 16 bit values on stack
; Leaves result on the stack
;
; Uses: HL, DE, BC, AF

i_or16:
	POP HL

	POP BC
	POP DE

	LD A, B
	OR D
	LD B, A

	LD A, C
	OR E
	LD C, A

	PUSH BC

	JP (HL)
