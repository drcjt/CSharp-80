; This routine performs the operation HL = -HL
;
; Uses: HL, AF, BC

s_neg:	
	POP BC		; Save return address

	POP HL

	LD A, L		; Negate HL
	CPL
	LD L, A
	LD A, H
	CPL
	LD H, A
	INC HL

	PUSH HL		; Put result back on stack

	PUSH BC		; Restore return address
	RET
