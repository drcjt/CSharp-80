SETXY:
	POP BC	; return address

	POP HL	 ; y
	POP DE   ; ignore msw

	POP DE	 ; x

	ADD HL, HL ; multiply y by 64
	ADD HL, HL
	ADD HL, HL
	ADD HL, HL
	ADD HL, HL
	ADD HL, HL
	ADD HL, DE ; add x
	LD DE, 3C00H
	ADD HL, DE

	POP DE	; ignore msw of x

	LD (4020H), HL	

	PUSH BC
	RET