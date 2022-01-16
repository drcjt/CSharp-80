; This routine performs the operation DEHL = DEHL >> C
;
; Uses: HL, DE, BC, AF, AF'


i_rsh:	
	POP IY

	POP HL
	POP DE

	POP BC

	LD A, C

	OR A
	JR Z, i_rsh_end

	LD B, A
	LD A, E

i_rsh_loop:

	SRL D
	RRA
	RR H
	RR L

	DJNZ i_lsh_loop

	LD E, A

i_rsh_end:

	POP BC

	PUSH DE		; Put result back on stack
	PUSH HL

	JP (IY)
