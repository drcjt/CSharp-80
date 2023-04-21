; This routine performs the operation DEHL = DEHL << C
;
; Uses: HL, HL', DE, BC, AF


i_lsh:	
	POP HL
	EXX

	POP HL
	POP DE

	POP BC

	LD A, C

	OR A
	JR Z, i_lsh_end

	LD B, A
	LD A, E

i_lsh_loop:

	ADD HL, HL
	RLA
	RL D

	DJNZ i_lsh_loop

	LD E, A

i_lsh_end:

	POP BC

	PUSH DE		; Put result back on stack
	PUSH HL

	EXX
	JP (HL)
