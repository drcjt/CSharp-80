; 16 bit less than comparison
; Args from Stack, HL < DE
; Carry set if true

i_lt16:
	POP BC

	POP DE
	POP HL

	AND A			; Clear zero flag
	SBC HL, DE

	JR C, i_lt16_1

	JR Z, i_lt16_1

	SCF				; set carry flag

	JP i_lt16_2

i_lt16_1:
	XOR A			; Clear carry flag

i_lt16_2:
	PUSH BC
	RET
