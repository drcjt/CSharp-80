; This routine performs the operation DEHL = DEHL - BCAF
;
; Uses: HL, DE, BC, AF, AF'

i_sub:	
	POP IY		; Save return address

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

	JP (IY)
