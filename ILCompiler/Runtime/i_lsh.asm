; This routine performs the operation DEHL = DEHL >> C
;
; Uses: HL, DE, BC, AF, AF'


i_rsh:	
	POP AF		; Save return address
	EX AF, AF'

	POP BC
	POP BC

	POP DE
	POP HL

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

	PUSH HL		; Put result back on stack
	PUSH DE

	EX AF, AF'	; Restore return address
	PUSH AF

	RET
