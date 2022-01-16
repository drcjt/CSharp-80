; This routine widens a 16 bit signed int to a 32 bit signed int
;
; Uses: HL, DE, BC


stoi:	
	POP IY		; Save return address

	POP DE

	LD H, D
	ADD HL, HL
	SBC HL, HL

	PUSH HL
	PUSH DE

	JP (IY)
