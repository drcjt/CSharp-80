TICKS:
	POP BC	; return address

	LD HL, 0
	PUSH HL

	; Push ticks as 32 bit number onto stack
	LD HL, (5C78H)	; lower two bytes of the frame counter increment once every 20ms
	PUSH HL

	PUSH BC
	RET
