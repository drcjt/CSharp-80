GETXY:
	POP BC	; return address

	LD HL, 0
	PUSH HL

	LD A, (5C88)
	NEG
	ADD 33

	LD H, A

	LD A, (5C89)
	NEG
	ADD 24

	LD L, A

	PUSH HL

	PUSH BC
	RET
