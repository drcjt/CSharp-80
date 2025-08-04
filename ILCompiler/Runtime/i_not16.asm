; Logical Not of one 16 bit values on stack
; Leaves result on the stack
;
; Uses: HL, BC, AF

i_not16:
	POP HL  ; Save return address

	POP BC

	LD A, C
	CPL
	LD C, A

	LD A, B
	CPL
	LD B, A
   
	PUSH BC

	JP (HL)
