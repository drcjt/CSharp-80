; 16 bit signed less than comparison
; Args from Stack, HL < DE
; Carry set if true

i_lt_un16:
	POP BC

	POP DE
	POP HL

	AND A			; Clear zero flag
	SBC HL, DE

	JR C, i_lt_un16_1

	JR Z, i_lt_un16_1

	SCF				; set carry flag

	JP i_lt_un16_2

i_lt_un16_1:
	XOR A			; Clear carry flag

i_lt_un16_2:
	PUSH BC
	RET
