; This routine performs the operation DEHL = DEHL + BCAF
;
; Uses: HL, HL', DE, BC, AF


i_add:
	POP HL		; Save return address
	EXX

	POP HL		; LSW first
	POP DE		; MSW next

	POP BC
	ADD HL, BC	; Add LSW
	EX DE, HL

	POP BC		; Add MSW
	ADC HL, BC

				; Put result back on stack
	PUSH HL		; MSW first
	PUSH DE		; LSW next

	EXX
	JP (HL)
