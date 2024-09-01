; 32 bit signed greater than comparison
; Entry: primary, secondary on stack
; Exit: Carry set if true

i_gt:
	POP HL
	EXX

	POP DE
	POP BC

	POP HL

	XOR A			; Clear carry flag
	SBC HL, DE

	EX DE, HL

	POP HL
	SBC HL, BC

	LD A, H
	ADD A, A

	EXX
	JP (HL)
