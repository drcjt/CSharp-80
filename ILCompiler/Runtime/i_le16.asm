; 16 bit less than or equal comparison
; Args from Stack, HL <= DE
; Carry set if true

i_le16:
	POP BC

	POP DE
	POP HL

	AND A			; Clear zero flag
	SBC HL, DE

	JR C, i_le16_1

	SCF				; set carry flag

	JP i_le16_2

i_le16_1:
	XOR A			; Clear carry flag

i_le16_2:
	PUSH BC
	RET
