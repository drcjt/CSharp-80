GetZero:
	POP HL
	LD DE, 0
	PUSH DE
	PUSH DE
	JP (HL)

GetValue:
	POP HL
	JP (HL)