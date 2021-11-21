; This routine performs the operation DEHL = DEHL << C
;
; Uses: HL, DE, BC, AF, AF'


i_lsh:	
	POP AF		; Save return address
	EX AF, AF'

	POP BC
	POP BC

	POP DE
	POP HL

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

	PUSH HL		; Put result back on stack
	PUSH DE

	EX AF, AF'	; Restore return address
	PUSH AF

	RET
