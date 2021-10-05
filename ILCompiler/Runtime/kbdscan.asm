KBDSCAN:
	POP BC	; return address

	CALL 2BH

	LD H, 0
	LD L, A		; The key that was pressed if any
	LD DE, 0

	PUSH HL
	PUSH DE

	PUSH BC
	RET
