; 16 bit unsigned greater than or equal comparison
; Args from Stack, HL >= DE
; Carry set if true

i_ge16_un:
	POP BC

	POP HL
	POP DE

	AND A			; Clear carry flag
	SBC HL, DE

	JR C, i_ge16_un_1	; value1 >= value2

	SCF				; set carry flag

	JP i_ge16_un_2

i_ge16_un_1:
	XOR A			; Clear carry flag

i_ge16_un_2:
	PUSH BC
	RET