; This routine performs the operation HL = HL - DE
;
; Uses: HL, DE, BC, AF

s_sub:	
	POP BC		; Save return address

	POP DE
	POP HL

	LD A, E		; Negate DE
	CPL
	LD E, A
	LD A, D
	CPL
	LD D, A
	INC DE

	ADD HL, DE

	PUSH HL

	PUSH BC		; Restore return address
	RET
