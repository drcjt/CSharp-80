; This routine performs the operation DEHL = DEHL + BCAF
;
; Uses: HL, HL', DE, BC, AF


i_add:
	EXX			; Save return address
	POP HL
	EXX

	POP HL		; LSW first
	POP DE		; MSW next

	POP BC
	ADD HL, BC	; Add LSW
	EX DE, HL

	POP BC		; Add MSW
	ADC HL, BC
	EX DE, HL	

				; Put result back on stack
	PUSH DE		; MSW first
	PUSH HL		; LSW next

	EXX
	PUSH HL
	EXX
	RET
