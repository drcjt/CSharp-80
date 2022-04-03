CLS:
	ld	de, clsdata
	ld	c, 9	
	jp	5	; let bdos return for us
clsdata:	db      1bh,"[0m",1bh,"[;H",1bh,"[2J$"
PRINT:
	LD A, (HL)
	CP 0
	JR Z, PRINTEND

	PUSH BC
	PUSH DE
	PUSH HL

	LD C, 2
	LD E, A
	CALL 5	; CALL bdos

	POP HL
	POP DE
	POP BC

	INC HL
	JR PRINT
PRINTEND:
	RET
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
