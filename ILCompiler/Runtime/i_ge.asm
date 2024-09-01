; 32 bit signed greater than equal comparison
; Entry: primary, secondary on stack
; Exit: Carry set if true
i_ge:
	POP BC
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
	JR C, i_ge_1

	LD A, H
	OR L
	OR D
	OR E

i_ge_1:
	EXX
	PUSH BC

	RET C

	SCF 
	RET Z

	CCF
	RET
