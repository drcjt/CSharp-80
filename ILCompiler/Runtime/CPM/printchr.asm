PRINTCHR:
	PUSH BC
	PUSH DE
	PUSH HL

	LD C, 2
	LD E, A
	CALL 5	; CALL bdos

	POP HL
	POP DE
	POP BC
	RET
