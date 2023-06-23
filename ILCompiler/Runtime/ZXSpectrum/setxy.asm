SETXY:
	POP BC	; return address

	POP HL	 ; y
	POP DE	 ; x

	PUSH BC

	ld a, 22
	rst 0x10

	ld a, l
	rst 0x10

	ld a, e
	rst 0x10

	RET
