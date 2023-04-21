; Logical Not of one 32 bit values on stack
; Leaves result on the stack
;
; Uses: HL, DE, BC, AF

i_not:
	POP HL

	POP BC
	POP DE

	LD A, C
	CPL
	LD C, A

	LD A, B
	CPL
	LD B, A

	LD A, E
	CPL
	LD E, A

	LD A, D
	CPL
	LD D, A
   
	PUSH DE
	PUSH BC

	JP (HL)
