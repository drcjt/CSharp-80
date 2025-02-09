; 16 bit signed greater than comparison
; Args from Stack, HL > DE
; Carry set if true

i_gt16:
	POP HL

	POP DE		; min
	POP BC		; one

	LD A, C
	SUB A, E
	LD A, B
	SBC A, D

	JP PO, $+5
	XOR A, 0x80
	JP M, i_gt16_1

	AND A
	JP (HL)

i_gt16_1:
	SCF
	JP (HL)
