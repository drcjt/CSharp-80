; 16 bit signed equal comparison
; Args from Stack, DEHL == BCAF
; Carry set if true

i_eq16:
	POP BC

	POP HL
	POP DE

	AND A			; Clear zero flag
	SBC HL, DE
	JR NZ, i_eq16_1

	SCF				; set carry flag

	JP i_eq16_2

i_eq16_1:
	XOR A			; Clear carry flag

i_eq16_2:
	PUSH BC
	RET