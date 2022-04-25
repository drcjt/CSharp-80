; Logical Or of two 16 bit values on stack
; Leaves result on the stack
;
; Uses: HL, DE, BC, AF

i_or16:
	POP BC

	POP HL
	POP DE

	LD A, H
	OR D
	LD H, A

	LD A, L
	OR E
	LD L, A

	PUSH HL

	PUSH BC
	RET
