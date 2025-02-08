; 32 bit signed greater than equal comparison
; Result = primary >= secondary
; Entry: primary, secondary on stack
; Exit: Carry set if true

i_ge:
	POP HL
	EXX

	POP DE		; primary
	POP BC

	POP HL		; secondary LSW

	LD A, E
	SUB A, L
	LD A, D
	SBC A, H

	POP HL

	LD A, C
	SBC A, L
	LD A, B
	SBC A, H
	
	JP PO, $+5
	XOR A, 0x80
	JP M, i_ge_1

	EXX

	SCF

	JP (HL)

i_ge_1:
	EXX

	AND A

	JP (HL)
