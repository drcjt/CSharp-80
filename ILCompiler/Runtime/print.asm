PRINT:
	LD E, (HL)	; Get string length into DE
	INC HL
	LD D, (HL)
	INC HL

	LD B, E		; Mystery fast loop calculus
	DEC DE
	INC D

PRINTLOOP:
	LD A, (HL)
	CALL PRINTCHR
	INC HL			; Chars are utf-16 so skip 2 bytes
	INC HL

	DJNZ PRINTLOOP
	DEC D
	JP NZ, PRINTLOOP

	RET
