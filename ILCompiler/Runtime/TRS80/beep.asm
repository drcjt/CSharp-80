BEEP:
	POP BC		; return address

	EXX 

	POP BC		; Duration
	POP DE

	POP DE		; Frequency
	POP HL

	; Freq in DE
	; Duration in BC

	DI

	PUSH IX

	DEC BC
	PUSH BC
	POP IX
	LD BC, -1
BEEP1:
	LD L, E
	LD H, D
	LD A, 1
	OUT (0FFH), A
BEEP2:
	ADD HL, BC
	JP C, BEEP2
	LD L, E
	LD H, D
	LD A, 2
	OUT (0FFH), A
BEEP3:
	ADD HL, BC
	JR C, BEEP3
	ADD IX, BC
	JP C, BEEP1

	POP IX

	EI

	EXX

	PUSH BC
	RET