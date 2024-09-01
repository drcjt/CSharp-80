; 32 bit unsigned greater than comparison
; Entry: primary, secondary on stack
; Exit: Carry set if true
i_gt_un:
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

	EXX
	JP (HL)