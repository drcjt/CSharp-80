SETXY:
	POP BC	; return address

	POP HL	 ; y
	POP DE   ; ignore msw

	POP DE	 ; x

	LD H, E	; HL = x, y

	POP DE	; ignore msw of x

	ld a, 22
	rst 2

	ld a, l
	rst 2

	ld a, h
	rst 2

	PUSH BC
	RET
