PRINT:
	LD BC, 2		; Add base size
	ADD HL, BC

	LD E, (HL)	; Get string length into DE
	INC HL
	LD D, (HL)
	INC HL

	LD A, D
	OR E
	JR Z, PRINTEND	; bail if string is 0 length

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

PRINTEND:
	RET
