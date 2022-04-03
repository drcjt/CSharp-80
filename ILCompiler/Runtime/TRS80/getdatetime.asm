GETDATETIME:
	POP BC

	POP HL			; Get address of return buffer

	PUSH IX			; Save IX

	PUSH HL
	POP IX

	LD A, (4045H)	; Get day
	LD (IX+0), A
	LD (IX+1), 0
	LD (IX+2), 0
	LD (IX+3), 0

	LD A, (4043H)	; Get hour
	LD (IX+4), A
	LD (IX+5), 0
	LD (IX+6), 0
	LD (IX+7), 0

	LD A, (4042H)	; Get minute
	LD (IX+8), A
	LD (IX+9), 0
	LD (IX+10), 0
	LD (IX+11), 0

	LD A, (4041H)	; Get second
	LD (IX+12), A
	LD (IX+13), 0
	LD (IX+14), 0
	LD (IX+15), 0

	POP IX

	PUSH BC
	RET