PRINT:
	LD A, (HL)
	CP 0
	JR Z, PRINTEND
	RST 2
	INC HL
	JR PRINT
PRINTEND:
	RET