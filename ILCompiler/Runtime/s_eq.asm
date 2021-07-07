; 16 bit signed equal comparison
; DE == HL
; Carry set if true
s_eq:
	POP BC

	POP HL
	POP DE

	OR A
	SBC HL, DE
	SCF
	INC HL
	RET Z
	XOR A
	LD L, A
	LD H, A

	PUSH BC

	RET
