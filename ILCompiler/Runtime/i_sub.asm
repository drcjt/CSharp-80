; This routine performs the operation DEHL = DEHL - BCAF
;
; Uses: HL, DE, BC, AF, AF'

i_sub:	
	POP AF		; Save return address
	EX AF, AF'

	POP BC
	POP AF
	POP DE
	POP HL

	PUSH BC
	PUSH AF

	OR A 

	POP BC
	SBC HL, BC

	POP BC
	EX DE, HL
	SBC HL, BC
	EX DE, HL

	PUSH HL		; Put result back on stack
	PUSH DE

	EX AF, AF'	; Restore return address
	PUSH AF

	RET
