; This routine performs the operation DEHL = DEHL + BCAF
;
; Uses: HL, DE, BC, AF, AF'


i_add:	
	POP AF		; Save return address
	EX AF, AF'

	POP DE		;DEHL
	POP HL
	POP BC		;BCAF
	POP AF

	PUSH BC		; Put BC back
	PUSH AF		; Swap AF and BC
	POP BC

	ADD HL, BC	; Add LSW
	EX DE, HL

	POP BC		; Add MSW
	ADC HL, BC
	EX DE, HL	

	PUSH HL		; Put result back on stack
	PUSH DE

	EX AF, AF'	; Restore return address
	PUSH AF

	RET
