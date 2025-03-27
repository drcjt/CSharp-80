; 32 bit unsigned greater than equal comparison
; Result = primary >= secondary
; Entry: primary, secondary on stack
; Exit: Carry set if true

i_ge_un:
	POP HL
	EXX

	POP HL		; primary
	POP DE

	AND A			; Clear carry flag

	POP BC		; secondary LSW
	SBC HL, BC
	EX DE, HL
	POP BC
	SBC HL, BC

	JR C, i_ge_un_1

	SCF

	JP i_ge_un_2
	
i_ge_un_1:
	XOR A

i_ge_un_2:
	EXX
	JP (HL)