; This routine performs the operation DEHL = DEHL - BCAF
;
; Uses: HL, DE, BC, AF, AF'

i_sub:	
	POP AF		; Save return address
	EX AF, AF'

	POP DE		;DEHL
	POP HL
	POP BC		;BCAF
	POP AF

	PUSH HL		; Save DEHL
	PUSH DE

	PUSH AF		; Put BCAF into DEHL
	PUSH BC
	POP DE
	POP HL

	LD A, L		; Negate DEHL which is really DEHL
	CPL
	LD L, A

	Ld A, H
	CPL
	LD H, A

	LD A, E
	CPL
	LD E, A

	LD A, D
	CPL
	LD D, A
	
	INC L
	JP NZ, i_neg_end
	INC H
	JP NZ, i_neg_end
	INC DE

i_neg_end:
	PUSH DE		; Put DEHL into BCAF
	PUSH HL
	POP AF
	POP BC

	POP DE		; Restore DEHL
	POP HL

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
