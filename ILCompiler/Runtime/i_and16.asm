; Logical And of two 16 bit values on stack
; Leaves result on the stack
;
; Uses: HL, DE, BC, AF

i_and16:
	POP HL

	POP BC
	POP DE

	LD A, B
	AND D
	LD B, A

	LD A, C
	AND E
	LD C, A

	PUSH BC
	
	JP (HL)
