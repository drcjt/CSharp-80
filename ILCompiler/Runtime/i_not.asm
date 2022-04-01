; Logical Not of one 32 bit values on stack
; Leaves result on the stack
;
; Uses: HL, DE, BC, AF

i_not:
	POP BC

	POP HL
	POP DE

	LD A, L
	CPL
	LD L, A

	LD A, H
	CPL
	LD H, A

	LD A, E
	CPL
	LD E, A

	LD A, D
	CPL
	LD D, A
   
	PUSH DE
	PUSH HL

	PUSH BC
	RET
