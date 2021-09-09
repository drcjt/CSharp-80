SETXY:
	POP BC   ; return address

	POP HL
	POP HL   ; y

	POP DE
	POP DE   ; x

	ADD HL, HL ; multiply y by 64
	ADD HL, HL
	ADD HL, HL
	ADD HL, HL
	ADD HL, HL
	ADD HL, HL
	ADD HL, DE ; add x
	LD DE, 3C00H
	ADD HL, DE

	PUSH BC  ; restore return address

	LD (4020H), HL	

	RET