KBDSCAN:
	;CALL 35BH
	;CALL 2BH
	;CALL 3E3H

	POP BC	; return address

	CALL 2BH

	LD HL, 0
	PUSH HL
	
	LD H, 0
	LD L, A		; The key that was pressed if any
	PUSH HL

	PUSH BC
	RET
