TICKS:
	POP BC	; return address

	; Push ticks as 32 bit number onto stack
	LD HL, (4040H)	; 25ms hearbeat counter on TRS-80 Model 1
	LD DE, 0
	PUSH HL
	PUSH DE

	PUSH BC
	RET
