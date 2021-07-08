; This routine widens a 16 bit signed int to a 32 bit signed int
;
; Uses: HL, DE, BC


stoi:	
	POP BC		; Save return address

	POP HL

	LD A, H
	AND 80H
	LD D, A
	LD E, 0
	LD A, H
	AND 7FH
	LD H, A

	PUSH HL
	PUSH DE

	PUSH BC		; Restore return address
	RET
