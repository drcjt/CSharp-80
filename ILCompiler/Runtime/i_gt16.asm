; 16 bit greater than comparison
; Args from Stack, HL > DE
; Carry set if true

i_gt16:
	POP BC

	POP HL
	POP DE

	AND A			; Clear zero flag
	SBC HL, DE

	JR C, i_gt16_1

	JR Z, i_gt16_1

	SCF				; set carry flag

	JP i_gt16_2

i_gt16_1:
	XOR A			; Clear carry flag

i_gt16_2:
	PUSH BC
	RET
