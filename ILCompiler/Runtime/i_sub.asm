; This routine performs the operation DEHL = DEHL - BCAF
;
; Uses: HL, HL', DE, BC, AF

i_sub:	
	EXX			; Save return address
	POP HL
	EXX

	POP HL		; LSW
	POP DE		; MSW

	OR A		; Clear carry
	POP BC		; LSW
	SBC HL, BC

	POP BC		; MSW
	EX DE, HL
	SBC HL, BC
	EX DE, HL

	; Put result back on stack
	PUSH DE		; MSW
	PUSH HL		; LSW

	EXX
	PUSH HL
	EXX
	RET
