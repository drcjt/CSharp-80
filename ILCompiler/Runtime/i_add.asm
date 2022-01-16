; This routine performs the operation DEHL = DEHL + BCAF
;
; Uses: HL, DE, BC, AF, AF'


i_add:	
	POP IY		; Save return address

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

	JP (IY)