; This routine performs the operation HL = HL + DE
;
; Uses: HL, DE, BC


s_add:	
	POP BC		; Save return address

	POP DE
	POP HL
	ADD HL, DE

	PUSH HL		; Put result back on stack

	PUSH BC		; Restore return address
	RET
