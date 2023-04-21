; This routine performs the operation DEHL = DEHL - BCAF
;
; Uses: HL, HL', DE, BC, AF

i_sub:	
	POP HL		; Save return address
	EXX

	POP HL		; LSW
	POP DE		; MSW

	OR A		; Clear carry
	POP BC		; LSW
	SBC HL, BC

	POP BC		; MSW
	EX DE, HL
	SBC HL, BC

	; Put result back on stack
	PUSH HL		; MSW
	PUSH DE		; LSW

	EXX
	JP (HL)
