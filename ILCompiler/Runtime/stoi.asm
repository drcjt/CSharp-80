; This routine widens a 16 bit signed int to a 32 bit signed int
;
; Uses: HL, DE, BC


stoi:	
	POP BC		; Save return address

	POP DE

	LD H, D
	ADD HL, HL
	SBC HL, HL

	PUSH DE
	PUSH HL

	PUSH BC		; Restore return address
	RET
