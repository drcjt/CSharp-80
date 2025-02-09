; 16 bit unsigned greater than comparison
; Args from Stack, HL > DE
; Carry set if true

i_gt16_un:
	POP BC

	POP HL
	POP DE

	AND A			; Clear zero flag
	SBC HL, DE

	JR C, i_gt16_un_1

	JR Z, i_gt16_un_1

	SCF				; set carry flag

	JP i_gt16_un_2

i_gt16_un_1:
	XOR A			; Clear carry flag

i_gt16_un_2:
	PUSH BC
	RET