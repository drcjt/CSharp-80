; Logical And of two 16 bit values on stack
; Leaves result on the stack
;
; Uses: HL, DE, BC, AF

i_and16:
	POP BC

	POP HL
	POP DE

	LD A, H
	AND D
	LD H, A

	LD A, L
	AND E
	LD L, A

	PUSH HL
	
	PUSH BC
	RET