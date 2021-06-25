SETXY:
	POP HL   ; return address
	POP BC   ; y 
	POP DE   ; x
	PUSH HL  ; restore return address
	LD HL, 4020H ; 
	LD (HL), E
	INC HL
	LD (HL), C
	RET
