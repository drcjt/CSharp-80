TICKS:
	POP BC	; return address

	LD HL, 0
	PUSH HL

	; Push ticks as 32 bit number onto stack
	LD HL, (4040H)	; 25ms hearbeat counter on TRS-80 Model 1
	PUSH HL

	PUSH BC
	RET
