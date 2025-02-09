; 16 bit signed greater than or equal comparison
; Args from Stack, HL >= DE
; Carry set if true

i_ge16:
	POP HL

	POP BC
	POP DE

	LD A, C
	SUB A, E
	LD A, B
	SBC A, D

	JP PO, $+5
	XOR A, 0x80
	JP M, i_ge16_1

	SCF
	JP (HL)

i_ge16_1:
	AND A
	JP (HL)
