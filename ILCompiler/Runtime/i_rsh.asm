; This routine performs the operation DEHL = DEHL >> C
;
; Uses: HL, HL', DE, BC, AF


i_rsh:	
	POP HL
	EXX

	POP HL
	POP DE

	POP BC

	LD A, C

	OR A
	JR Z, i_rsh_end

	LD B, A
	LD A, E

i_rsh_loop:

	SRA D
	RRA
	RR H
	RR L

	DJNZ i_rsh_loop

	LD E, A

i_rsh_end:

	POP BC

	PUSH DE		; Put result back on stack
	PUSH HL

	EXX 
	JP (HL)
