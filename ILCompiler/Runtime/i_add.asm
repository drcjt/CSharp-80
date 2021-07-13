; This routine performs the operation DEHL = DEHL + BCAF
;
; Uses: HL, DE, BC, AF, AF'


i_add:	
	POP AF		; Save return address
	EX AF, AF'

	POP BC
	POP AF

	POP DE
	POP HL

	PUSH BC		; Put BC back
	PUSH AF		; Swap AF and BC

	OR A

	POP BC
	ADC HL, BC	; Add LSW

	POP BC		; Add MSW
	EX DE, HL
	ADC HL, BC
	EX DE, HL	

	PUSH HL		; Put result back on stack
	PUSH DE

	EX AF, AF'	; Restore return address
	PUSH AF

	RET
