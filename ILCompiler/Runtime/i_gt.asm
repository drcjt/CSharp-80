; 32 bit signed greater than comparison
; Entry: primary, secondary on stack
; Exit: Carry set if true

i_gt:
	POP HL
	EXX

	POP DE
	POP BC

	POP HL

	LD A, L
	SUB A, E
	LD A, H
	SBC A, D

	POP HL

	LD A, L
	SBC A, C
	LD A, H
	SBC A, B

	JP PO, $+5
	XOR A, 0x80
	JP M, i_gt_1

	EXX

	AND A

	JP (HL)

i_gt_1:
	EXX

	SCF

	JP (HL)
