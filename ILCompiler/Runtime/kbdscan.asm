KBDSCAN:
	POP IY	; return address

	CALL 2BH

	LD HL, 0
	PUSH HL
	
	LD H, 0
	LD L, A		; The key that was pressed if any
	PUSH HL

	JP (IY)
