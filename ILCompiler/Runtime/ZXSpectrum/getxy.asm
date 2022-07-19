GETXY:
	POP BC	; return address

	LD HL, 0
	PUSH HL

	LD A, (5C88H)
	NEG
	ADD 33

	LD H, A

	LD A, (5C89H)
	NEG
	ADD 24

	LD L, A

	PUSH HL

	PUSH BC
	RET
